using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.Messaging;
using AccurateAppend.Operations.Contracts;
using AccurateAppend.Sales;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.Security;
using AccurateAppend.Websites.Admin.Areas.Accounting.Data;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Areas.Sales.SendPaymentUpdate.Messages
{
    /// <summary>
    /// Handler for the <see cref="SendPaymentUpdateLinkCommand"/> and <see cref="SendPaymentExpiredLinkCommand"/> bus commands.
    /// </summary>
    /// <remarks>
    /// Responds to a message by crafting a tracked email request to the client.
    /// </remarks>
    public class EmailUpdateLinkCommandHandler : IHandleMessages<SendPaymentUpdateLinkCommand>,IHandleMessages<SendPaymentExpiredLinkCommand>
    {
        #region Fields

        private readonly DefaultContext dataContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailUpdateLinkCommandHandler"/> class.
        /// </summary>
        /// <param name="dataContext">The required <see cref="DefaultContext"/> component.</param>
        public EmailUpdateLinkCommandHandler(DefaultContext dataContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));

            this.dataContext = dataContext;
        }

        #endregion

        #region IHandleMessages<SendPaymentUpdateLinkCommand> Members

        /// <inheritdoc />
        public virtual async Task Handle(SendPaymentUpdateLinkCommand message, IMessageHandlerContext context)
        {
            Validator.ValidateObject(message, new ValidationContext(message));

            var site = SiteCache.Cache.First(s => s.ApplicationId == WellKnownIdentifiers.AccurateAppendId);

            using (context.Alias())
            {
                var order = await this.dataContext
                    .SetOf<TransactionEvent>()
                    .Where(c => c.Id == message.ChargeEventId)
                    .Select(c => c.Order)
                    .Include(o => o.Transactions)
                    .Include(o => o.Deal)
                    .Include(o => o.Deal.Client)
                    .FirstAsync()
                    .ConfigureAwait(false);

                var sendTo = await this.T(order.Deal.Client);

                var charge = order.Transactions.First(c => c.Id == message.ChargeEventId);

                // add note
                order.Deal.Notes.Add("Payment declined email sent");

                await this.dataContext.SaveChangesAsync();

                var formatter = new PaymentUpdateFormatter(site);
                var email = formatter.Create(charge, sendTo);

                // Send tracked email

                var command = new SendEmailCommand(email);
                command.Track = true;

                await context.Send(command);
            }
        }

        #endregion

        #region IHandleMessages<SendPaymentExpiredLinkCommand> Members

        /// <inheritdoc />
        public virtual async Task Handle(SendPaymentExpiredLinkCommand message, IMessageHandlerContext context)
        {
            Validator.ValidateObject(message, new ValidationContext(message));

            var site = SiteCache.Cache.First(s => s.ApplicationId == WellKnownIdentifiers.AccurateAppendId);

            using (context.Alias())
            {
                var client = await this.dataContext
                    .SetOf<ClientRef>()
                    .Where(c => c.UserId == message.UserId)
                    .AsNoTracking()
                    .FirstAsync()
                    .ConfigureAwait(false);

                var sendTo = await this.T(client);

                var formatter = new PaymentUpdateFormatter(site);
                var email = formatter.Create(client, sendTo);

                // add note
                const String Sql = @"
INSERT INTO [operations].[Notes] VALUES (GETUTCDATE(), 'Payment expired email sent', @p0)
DECLARE @NoteId int
SELECT @NoteId = SCOPE_IDENTITY()

DECLARE @UserDetailId int
SELECT @UserDetailId = [UserDetailId] FROM [accounts].[UserDetail] WHERE [UserId] = @p1
INSERT INTO [accounts].[UserDetailNotes] VALUES (@UserDetailId, @NoteId)";

                await this.dataContext.Database.ExecuteSqlCommandAsync(Sql, context.InitiatingUserId(), client.UserId);
                
                // Send tracked email
                var command = new SendEmailCommand(email);
                command.Track = true;

                await context.Send(command);
            }
        }

        #endregion

        #region Helpers

        protected virtual async Task<IEnumerable<MailAddress>> T(ClientRef client)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            var sendTo = await this.dataContext
                .Database
                .SqlQuery<String>(@"
SELECT [EmailAddress] FROM [accounts].[ClientContacts] c (nolock)
INNER JOIN [accounts].[UserDetail] (nolock) ud ON ud.[UserDetailId]=c.[UserDetailId]
WHERE [UserId] = @p0 AND [Billing]=1", client.UserId)
                .ToListAsync();
            sendTo.Add(client.UserName);

            return sendTo.Distinct(StringComparer.OrdinalIgnoreCase).Select(e => new MailAddress(e));
        }

        #endregion
    }
}