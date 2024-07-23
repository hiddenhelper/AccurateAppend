using AccurateAppend.Accounting;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using EventLogger;
using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Mvc;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Security;
using AccurateAppend.Standardization;
using AccurateAppend.Websites.Clients.Areas.Profile.Contact.Models;
using Application = AccurateAppend.Core.Definitions.Application;

namespace AccurateAppend.Websites.Clients.Areas.Profile.Contact
{
    /// <summary>
    /// Controller to allow an interactive client to update their contact information.
    /// </summary>
    [Authorize()]
    public class Controller : System.Web.Mvc.Controller
    {
        #region Fields

        private readonly Accounting.DataAccess.DefaultContext context;
        private readonly IAddressStandardizer addressParser;
        private readonly INameStandardizer nameParser;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        /// <param name="context">The <see cref="Accounting.DataAccess.DefaultContext"/> component providing data access to this instance.</param>
        /// <param name="addressParser">The <see cref="IAddressStandardizer"/> component.</param>
        /// <param name="nameParser">The <see cref="INameStandardizer"/> component.</param>
        public Controller(Accounting.DataAccess.DefaultContext context, IAddressStandardizer addressParser, INameStandardizer nameParser)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (addressParser == null) throw new ArgumentNullException(nameof(addressParser));
            if (nameParser == null) throw new ArgumentNullException(nameof(nameParser));
            Contract.EndContractBlock();

            this.context = context;
            this.addressParser = addressParser;
            this.nameParser = nameParser;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Action to display the contact profile form.
        /// </summary>
        [HttpGet()]
        public virtual async Task<ActionResult> Index(CancellationToken cancellation)
        {
            try
            {
                using (this.context.CreateScope(ScopeOptions.ReadOnly))
                {
                    var client = await context.SetOf<Client>().ForInteractiveUser().FirstAsync(cancellation);
                    var model = new ContactDetailsModel
                    {
                        BusinessName = client.BusinessName,
                        
                        FirstName = client.FirstName,
                        LastName = client.LastName,
                        Address = client.Address.Address,
                        City = client.Address.City,
                        State = client.Address.State,
                        PostalCode = client.Address.Zip,
                        Phone = client.PrimaryPhone.Value,
                        Email = client.DefaultEmail,
                        Country = client.Address.Country
                    };

                    // prevent No Name from showing in Profile/Contact form
                    if (PartyExtensions.IsUnknownName(client.FirstName, client.LastName))
                    {
                        model.FirstName = String.Empty;
                        model.LastName = String.Empty;
                    }

                    return this.View(model);
                }
            }
            catch (Exception ex)
            {
                Logger.LogEvent(ex, Severity.High, Application.Clients, this.Request.UserHostAddress, "Edit Contact failing.");
                return this.DisplayErrorResult("An error occurred and your details could not be loaded. Please contact customer support.");
            }
        }

        [HttpPost()]
        public virtual async Task<ActionResult> Index(ContactDetailsModel model, CancellationToken cancellation)
        {
            model = model ?? new ContactDetailsModel();
            if (!this.ModelState.IsValid) return this.View(model);

            model.CleanUp(this.addressParser, this.nameParser);
            
            try
                {
                    using (var uow = this.context.CreateScope(ScopeOptions.AutoCommit))
                    {
                        var client = await this.context
                            .SetOf<Client>()
                            .ForInteractiveUser()
                            .FirstAsync(cancellation);
                        client.BusinessName = model.BusinessName;
                        client.FirstName = model.FirstName;
                        client.LastName = model.LastName;
                        client.Address.Address = model.Address;
                        client.Address.City = model.City;
                        client.Address.State = model.State;
                        client.Address.Zip = model.PostalCode;
                        client.PrimaryPhone.Value = model.Phone;
                        client.Address.Country = model.Country;

                        if (client.DefaultEmail != model.Email)
                        {
                            // This section requires serialized transactions because we're changing a core username here
                            using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                            {
                                if (await this.AccountEmailExists(model.Email, cancellation))
                                {
                                    this.ModelState.AddModelError(nameof(ContactDetailsModel.Email), "This email address cannot be used for your account. Please contact customer support.");
                                    return this.View(model);
                                }

                                await uow.CommitAsync(cancellation);

                                await this.context.Database.ExecuteSqlCommandAsync("exec [accounts].[UpdateUserEmail] @p0, @p1", cancellation, this.User.Identity.GetIdentifier(), model.Email);
                                transaction.Complete();
                            }
                        }
                        else
                        {
                            await uow.CommitAsync(cancellation);
                        }
                    }

                    this.TempData["message"] = "Your contact information has been updated.";
                    this.TempData["messageType"] = "success";
                }
                catch (Exception ex)
                {
                    Logger.LogEvent(ex, Severity.High, Application.Clients, this.Request.UserHostAddress, "Edit Contact failing.");
                    return this.DisplayErrorResult("An error occurred and your contact details could not updated. Please contact customer support.");
                }

            return this.View(model);
        }

        #endregion

        #region Helpers

        protected virtual Task<Boolean> AccountEmailExists(String newEmailAddress, CancellationToken cancellation)
        {
            return this.context
                .SetOf<User>()
                .AnyAsync(u => u.UserName == newEmailAddress, cancellation);
        }

        #endregion
    }
}