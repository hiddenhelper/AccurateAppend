using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.ChargeProcessing;
using AccurateAppend.ChargeProcessing.Contracts;
using AccurateAppend.ChargeProcessing.DataAccess;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Data;
using AccurateAppend.Websites.Clients.Areas.Shared.Models;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Public.Payment
{
    /// <summary>
    /// Controller for managing public update of user credit cards.
    /// </summary>
    [AllowAnonymous()]
    public class Controller : System.Web.Mvc.Controller
    {
        #region Fields

        private readonly DefaultContext context;
        private readonly IEncryptor encryption;
        private readonly IMessageSession bus;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> instance used by the controller.</param>
        /// <param name="encryption">Provides CC encryption services.</param>
        /// <param name="bus">The <see cref="IMessageSession"/> used to capture the card data.</param>
        public Controller(DefaultContext context, IEncryptor encryption, IMessageSession bus)
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

        /// <summary>
        /// Replaces existing client's credit card.
        /// </summary>
        public async Task<ActionResult> Update(Guid userid, CancellationToken cancellation)
        {
            // Logged in users should always use the "real" application for this logic.
            if (this.User.Identity.IsAuthenticated) return this.RedirectToAction("Index", "Card", new { Area = "Profile" });

            var model = await this.context
                .Set<ClientRef>()
                .Where(u => u.UserId == userid)
                .Select(u => new PaymentDetailsModel() {UserId = u.UserId, ApplicationId = u.ApplicationId})
                .FirstOrDefaultAsync(cancellation);
            if (model == null) throw new InvalidOperationException($"The user '{userid}' could not be matched to a customer.");

            return this.View(model);
        }

        /// <summary>
        /// Replaces existing client's credit card.
        /// </summary>
        [HttpPost()]
        public async Task<ActionResult> Update(PaymentDetailsModel paymentDetails, CancellationToken cancellation)
        {
            this.TryValidateModel(paymentDetails);

            if (!this.ModelState.IsValid) return this.View(paymentDetails);

            try
            {
                var client = await this.context
                    .Set<ClientRef>()
                    .Where(u => u.UserId == paymentDetails.UserId)
                    .Select(u => new {u.UserId, u.UserName, u.ApplicationId})
                    .FirstAsync(cancellation);

                var address = new BillingAddressPayload();
                address.FirstName = paymentDetails.CardHolderFirstName;
                address.LastName = paymentDetails.CardHolderLastName;
                address.BusinessName = paymentDetails.CardHolderBusinessName;
                address.PhoneNumber = paymentDetails.CardHolderPhone;

                var card = new CreditCardPayload(this.encryption.SymetricEncrypt(paymentDetails.CardNumber), paymentDetails.GetExpirationDate(), paymentDetails.CardCvv);
                
                var command = new CreatePaymentProfileCommand
                {
                    UserId = paymentDetails.UserId,
                    Card = card,
                    BillingAddress = address,
                    RequestId = Guid.NewGuid() // This is fire and forget so we don't care
                };

                // The client isn't authenticated here so we needs to use a proxy identity in scope of sending the command
                using (SecurityHelper.Alias(SecurityHelper.CreateIdentity(client.UserName, client.UserId, client.ApplicationId)))
                {
                    await this.bus.Send(command);
                }

                return this.View("ThankYou");
            }
            catch (Exception ex)
            {
                EventLogger.Logger.LogEvent(ex, Severity.High, Application.Clients, this.Request.UserHostAddress, "Update credit card failing.");
                return this.View(paymentDetails);
            }
        }

        #endregion
    }
}