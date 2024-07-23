using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Transactions;
using AccurateAppend.DQS;
using AccurateAppend.Standardization;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Public.OptOut.Messaging
{
    /// <summary>
    /// Handler responsible for adding global suppression data for opt out requests.
    /// </summary>
    public class OptOutCommandHandler : IHandleMessages<OptOutCommand>
    {
        #region IHandleMessages<OptOutCommand> Members

        /// <inheritdoc />
        public virtual async Task Handle(OptOutCommand message, IMessageHandlerContext context)
        {
            Validator.ValidateObject(message, new ValidationContext(message));

            using (var transaction =
                new TransactionScope(TransactionScopeOption.Suppress, TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    await this.SuppressEmail(message.Email).ConfigureAwait(false);
                    await this.SuppressPhoneNumber(message.Phone).ConfigureAwait(false);
                    await this.SuppressParty(message.FirstName, message.LastName, message.Address, message.City,
                        message.State, message.PostalCode).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                transaction.Complete();
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Adds a globally filtered email address to the <see cref="DAL.Databases.ClientEmailSuppression"/>.
        /// </summary>
        protected virtual async Task SuppressEmail(String emailAddress)
        {
            if (String.IsNullOrWhiteSpace(emailAddress)) return;

            using (var db = new DAL.Databases.ClientEmailSuppression())
            {
                await db.AddGlobalFilter(emailAddress).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Adds a globally filtered phone number to the <see cref="DAL.Databases.PhoneSuppression"/>.
        /// </summary>
        protected virtual async Task SuppressPhoneNumber(String phone)
        {
            if (String.IsNullOrWhiteSpace(phone)) return;

            using (var p = ParserFactory.CreatePhoneStandardizer())
            {
                var result = p.Item.Parse(phone);

                using (var db = new DAL.Databases.PhoneSuppression())
                {
                    const String Sql = "INSERT INTO [dbo].[PhoneSuppression] VALUES (null, @p0)";

                    await db.Database.ExecuteSqlCommandAsync(Sql, result.PhoneNumber).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Adds a globally filtered party to the <see cref="DAL.Databases.SuppressionTokenCache"/>.
        /// </summary>
        protected virtual async Task SuppressParty(String firstName, String lastName, String street, String city, String state, String zip)
        {
            INameObjectResult name;
            IAddressObjectResult address;

            using (var p = ParserFactory.CreateNameStandardizer())
            {
                name = p.Item.Parse($"{firstName} {lastName}");
            }

            using (var p = ParserFactory.CreateAddressStandardizer())
            {
                address = p.Item.Parse(street, city, state, zip);
            }

            using (var db = new DAL.Databases.SuppressionTokenCache())
            {
                const String Sql = "INSERT INTO [dbo].[PartySuppression] ([FirstName],[LastName],[HouseNo],[PreDir],[StreetName],[StreetType],[PostDir],[City],[State],[Zip]) VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9)";

                await db.Database.ExecuteSqlCommandAsync(
                    Sql, 
                    name.FirstName.ToUpperInvariant(), 
                    name.LastName.ToUpperInvariant(), 
                    address.ParsedAddressRange.ToUpperInvariant(),
                    address.ParsedPreDirection.ToUpperInvariant(), 
                    address.ParsedStreetName.ToUpperInvariant(), 
                    address.ParsedSuffix.ToUpperInvariant(),
                    address.ParsedPostDirection.ToUpperInvariant(), 
                    address.City.ToUpperInvariant(), 
                    address.State.ToUpperInvariant(), 
                    address.Zip.ToUpperInvariant()).ConfigureAwait(false);
            }
        }

        #endregion
    }
}