using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.ChargeProcessing.Contracts;
using AccurateAppend.ChargeProcessing.DataAccess;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Websites.Clients.Areas.Profile.Card.Models;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Profile.Card
{
    /// <summary>
    /// Controller for adding client CC billing information.
    /// </summary>
    [Authorize()]
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
        /// <param name="context">The <see cref="DefaultContext"/> component providing data access to this instance.</param>
        /// <param name="encryption">Provides CC encryption services.</param>
        /// <param name="bus">The <see cref="IMessageSession"/> providing messaging access.</param>
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
        /// Action to display the CC entry form.
        /// </summary>
        [HttpGet()]
        public virtual ActionResult Index(CancellationToken cancellation)
        {
            return this.View(new PaymentDetailsModel()
            {
                UserId = this.User.Identity.GetIdentifier()
            });
        }

        [HttpPost()]
        public virtual async Task<ActionResult> Index(PaymentDetailsModel model, CancellationToken cancellation)
        {
            if (!this.ModelState.IsValid) return this.Json(new { Success = false, Message = "Validation error", Errors = this.ModelState.Values.SelectMany(v => v.Errors)});

            try
            {
                var address = new BillingAddressPayload();
                address.FirstName = model.CardHolderFirstName;
                address.LastName = model.CardHolderLastName;
                address.BusinessName = model.CardHolderBusinessName;
                address.PostalCode = model.CardPostalCode;
                address.PhoneNumber = model.CardHolderPhone;

                var card = new CreditCardPayload(this.encryption.SymetricEncrypt(model.CardNumber), model.GetExpirationDate(), model.CardCvv);
                
                var command = new CreatePaymentProfileCommand
                {
                    UserId = model.UserId,
                    Card = card,
                    BillingAddress = address,
                    RequestId = model.RequestId
                };

                await this.bus.Send(command);
                
                return this.Json(new {Success = true, Message = "Updating your payment information."});
            }
            catch (Exception ex)
            {
                EventLogger.Logger.LogEvent(ex, Severity.High, Application.Clients, this.Request.UserHostAddress, "Edit Payment failing.");
                return this.Json(
                    new
                    {
                        Success = false,
                        Message = "Server error",
                        Errors = new[]
                        {
                            new ModelError("An error occured and your payment details could not updated. Please contact customer support to update your payment information.")
                        }
                    });
            }
        }

        #endregion
    }
}