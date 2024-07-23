using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.ChargeProcessing;
using AccurateAppend.ChargeProcessing.Contracts;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Data;
using AccurateAppend.Websites.Admin.Areas.Billing.CreateCreditCard.Models;
using AccurateAppend.Websites.Admin.Areas.Billing.Shared.Models;
using AccurateAppend.Websites.Admin.Areas.Billing.ViewCreditCards;
using AccurateAppend.Websites.Admin.Navigator;
using EventLogger;
using NServiceBus;
using Application = AccurateAppend.Core.Definitions.Application;
using DefaultContext = AccurateAppend.ChargeProcessing.DataAccess.DefaultContext;

namespace AccurateAppend.Websites.Admin.Areas.Billing.CreateCreditCard
{
    /// <summary>
    /// Controller for adding a new payment account to a client.
    /// </summary>
    [Authorize()]
    public class CreateCreditCardController : Controller
    {
        #region Fields

        private readonly DefaultContext context;
        private readonly IEncryptor encryption;
        private readonly IMessageSession bus;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCreditCardController"/> class.
        /// </summary>
        public CreateCreditCardController(DefaultContext context, IEncryptor encryption, IMessageSession bus)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (encryption == null) throw new ArgumentNullException(nameof(encryption));
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            Contract.EndContractBlock();

            this.context = context;
            this.encryption = encryption;
            this.bus = bus;
        }

        #endregion

        #region Action Methods

        public async Task<ActionResult> Index(Guid userId, CancellationToken cancellation)
        {
            var user = await this.context
                .Set<ClientRef>()
                .Where(c => c.UserId == userId)
                .Select(c => new {c.UserId, c.UserName})
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellation);
            if (user == null) return this.DisplayErrorResult($"An error has occurred while creating the card. The user {userId} does not exist.");

            var model = new AddCreditCardModel()
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Address = new BillingAddressModel(),
                Card = new CreditCardModel()
            };

            return this.View(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual async Task<ActionResult> Index(AddCreditCardModel model, CancellationToken cancellation)
        {
            if (!this.ModelState.IsValid) return this.View("Index", model);

            try
            {
                if (!await this.context
                    .Set<ClientRef>()
                    .AnyAsync(c => c.UserId == model.UserId, cancellation))
                {
                    return this.DisplayErrorResult($"An error has occurred while creating the card. The user {model.UserId} does not exist.");
                }

                var card = new CreditCardPayload(model.Card.CardNumber, model.Card.CreateExpirationDate(), model.Card.CscValue);

                var address = new BillingAddressPayload();
                address.FirstName = model.Address.FirstName;
                address.LastName = model.Address.LastName;
                address.BusinessName = model.Address.BusinessName;
                address.Address = model.Address.Address;
                address.City = model.Address.City;
                address.State = model.Address.State;
                address.PostalCode = model.Address.Zip;
                address.Country = model.Address.Country;
                address.PhoneNumber = model.Address.PhoneNumber;

                var command = new CreatePaymentProfileCommand();
                command.Card = new CreditCardPayload(this.encryption.SymetricEncrypt(card.CardNumber), card.ExpirationDate, card.CscValue);

                command.BillingAddress = new BillingAddressPayload();
                command.BillingAddress.BusinessName = address.BusinessName;
                command.BillingAddress.Address = address.Address;
                command.BillingAddress.City = address.City;
                command.BillingAddress.State = address.State;
                command.BillingAddress.PostalCode = address.PostalCode;
                command.BillingAddress.Country = address.Country;
                command.BillingAddress.FirstName = address.FirstName;
                command.BillingAddress.LastName = address.LastName;
                command.BillingAddress.PhoneNumber = address.PhoneNumber;
                command.UserId = model.UserId;

                command.RequestId = Guid.NewGuid(); // Not used yet

                await this.bus.SendLocal(command);

                // Fake wait processing time because we're not using callbacks
                await Task.Delay(TimeSpan.FromSeconds(4), cancellation);

                return this.NavigationFor<ViewCreditCardsController>().Detail(model.UserId);
            }
            catch (Exception ex)
            {
                Logger.LogEvent(ex, Severity.High, Application.AccurateAppend_Admin, this.Request.UserHostAddress, this.User.Identity.Name);
                this.TempData["message"] = $"An error has occurred while creating the card. Message: {ex.Message}";

                return this.View("~/Views/Shared/Error.aspx");
            }
        }

        #endregion
    }
}