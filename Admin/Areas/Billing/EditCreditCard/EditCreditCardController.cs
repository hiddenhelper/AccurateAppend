using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.ChargeProcessing;
using AccurateAppend.ChargeProcessing.Contracts;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Data;
using AccurateAppend.Websites.Admin.Areas.Billing.EditCreditCard.Models;
using AccurateAppend.Websites.Admin.Areas.Billing.Shared.Models;
using AccurateAppend.Websites.Admin.Areas.Billing.ViewCreditCards;
using AccurateAppend.Websites.Admin.Navigator;
using EventLogger;
using Application = AccurateAppend.Core.Definitions.Application;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Controllers.Bases;
using DomainModel.ActionResults;
using Microsoft.AspNet.SignalR;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Areas.Billing.EditCreditCard
{
    /// <summary>
    /// Controller for editing an existing payment account billing address.
    /// </summary>
    [System.Web.Mvc.Authorize()]
    public class EditCreditCardController : Controller
    {
        #region Fields

        private readonly ChargeProcessing.DataAccess.DefaultContext context;
        private readonly IMessageSession bus;
        private readonly IEncryptor encryption;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EditCreditCardController"/> class.
        /// </summary>
        public EditCreditCardController(ChargeProcessing.DataAccess.DefaultContext context, IMessageSession bus, IEncryptor encryption)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            Contract.EndContractBlock();

            this.context = context;
            this.bus = bus;
            this.encryption = encryption;
        }

        #endregion

        #region Action Methods

        [AllowAnonymous]
        public ActionResult CimStart()
        {
            return this.View();
        }

        [AllowAnonymous]
        public ActionResult DisplayForm(String paymentId)
        {
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.PRODUCTION;
            // define the merchant information (authentication / transaction id)
            var merchantAuthentication = new merchantAuthenticationType()
            {
                name = this.encryption.SymetricDecrypt("AzsYM5sUYQKqWyrH+KbiSA=="),
                ItemElementName = ItemChoiceType.transactionKey,
                Item = this.encryption.SymetricDecrypt("gsnz5DLFz8USYZnQDRKyoMugHwGbzke5b4kf9r9q5no=")
            };

            var settings = new List<settingType>();

            settings.Add(new settingType());
            settings[0].settingName = settingNameEnum.hostedProfileReturnUrl.ToString();
            var p = this.Request.Url.Host == "localhost" ? "http" : "https";

            settings[0].settingValue = this.Url.Action("CimFinish", "EditCreditCard", new {customerId=1909051414,paymentId=1921369413}, p);

            settings.Add(new settingType());
            settings[1].settingName = settingNameEnum.hostedProfilePaymentOptions.ToString();
            settings[1].settingValue = "showCreditCard";

            settings.Add(new settingType());
            settings[2].settingName = settingNameEnum.hostedProfileValidationMode.ToString();
            settings[2].settingValue = "liveMode";

            settings.Add(new settingType());
            settings[3].settingName = settingNameEnum.hostedProfileBillingAddressRequired.ToString();
            settings[3].settingValue = "true";

            settings.Add(new settingType());
            settings[4].settingName = settingNameEnum.hostedProfileCardCodeRequired.ToString();
            settings[4].settingValue = "true";

            settings.Add(new settingType());
            settings[5].settingName = settingNameEnum.hostedProfileManageOptions.ToString();
            settings[5].settingValue = "showPayment";

            var request = new getHostedProfilePageRequest();
            request.merchantAuthentication = merchantAuthentication;
            request.customerProfileId = "1909051414";
            request.hostedProfileSettings = settings.ToArray();

            var controller = new getHostedProfilePageController(request);
            controller.Execute();

            var response = controller.GetApiResponse();
            if (response != null && response.messages.resultCode == messageTypeEnum.Ok)
            {
                Console.WriteLine(response.messages.message[0].code);
                Console.WriteLine(response.messages.message[0].text);
                Console.WriteLine("Token: " + response.token.ToString());
            }
            else if (response != null)
            {
                throw new Exception ("Error: " + response.messages.message[0].code + "  " +
                                              response.messages.message[0].text);
            }
            else
            {
                throw new Exception("Error: empty sponse");
            }
            return this.View((Object)response.token);
        }
        [AllowAnonymous]
        public ActionResult CimFinish(String customerId, String paymentId)
        {
            var sb = new StringBuilder();


            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.PRODUCTION;
            // define the merchant information (authentication / transaction id)
            var merchantAuthentication = new merchantAuthenticationType()
            {
                name = this.encryption.SymetricDecrypt("AzsYM5sUYQKqWyrH+KbiSA=="),
                ItemElementName = ItemChoiceType.transactionKey,
                Item = this.encryption.SymetricDecrypt("gsnz5DLFz8USYZnQDRKyoMugHwGbzke5b4kf9r9q5no=")
            };

            var request = new getCustomerPaymentProfileRequest();
            request.merchantAuthentication = merchantAuthentication;
            request.customerProfileId = customerId;
            request.customerPaymentProfileId = paymentId;

            // Set this optional property to true to return an unmasked expiration date
            request.unmaskExpirationDateSpecified = true;
            request.unmaskExpirationDate = true;


            // instantiate the controller that will call the service
            var controller = new getCustomerPaymentProfileController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            
            if (response != null && response.messages.resultCode == messageTypeEnum.Ok)
            {
                sb.AppendLine(response.messages.message[0].text);
                sb.AppendLine("Customer Payment Profile Id: " + response.paymentProfile.customerPaymentProfileId);
                sb.AppendLine("Bill to Phone: " + response.paymentProfile.billTo.phoneNumber);
                sb.AppendLine("Bill to Address: " + response.paymentProfile.billTo.address);
                sb.AppendLine("Bill to City: " + response.paymentProfile.billTo.city);
                sb.AppendLine("Bill to State: " + response.paymentProfile.billTo.state);
                sb.AppendLine("Bill to Zip: " + response.paymentProfile.billTo.zip);

                sb.AppendLine("Customer Payment Profile Id: " + response.paymentProfile.customerPaymentProfileId);
                if (response.paymentProfile.payment.Item is creditCardMaskedType)
                {
                    sb.AppendLine("Customer Payment Profile Last 4: " + (response.paymentProfile.payment.Item as creditCardMaskedType).cardNumber);
                    sb.AppendLine("Customer Payment Profile Expiration Date: " + (response.paymentProfile.payment.Item as creditCardMaskedType).expirationDate);

                    if (response.paymentProfile.subscriptionIds != null && response.paymentProfile.subscriptionIds.Length > 0)
                    {
                        sb.AppendLine("List of subscriptions : ");
                        for (int i = 0; i < response.paymentProfile.subscriptionIds.Length; i++)
                            sb.AppendLine(response.paymentProfile.subscriptionIds[i]);
                    }
                }
            }
            else if (response != null)
            {
                sb.AppendLine("Error: " + response.messages.message[0].code + "  " +
                                  response.messages.message[0].text);
            }
            var context = GlobalHost.ConnectionManager.GetHubContext<CallbackHub>();
            context.Clients.All.addNewMessageToPage(sb.ToString());

            return new LiteralResult(true) {Data = "done"};
        }

        public async Task<ActionResult> ChangeBillingAddress(Int32 cardId, CancellationToken cancellation)
        {
            var client = await this.context
                .Set<ChargePayment>()
                .Where(c => c.Id == cardId)
                .Select(c => new {c.Client.UserName, c.Client.UserId})
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellation);
            if (client == null) return this.DisplayErrorResult($"An error has occurred while updating the card. The card {cardId} does not exist.");

            var model = new ChangeBillingAddressModel
            {
                UserName = client.UserName,
                UserId = client.UserId,
                Address = new BillingAddressModel()
            };

            return this.View(model);
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual async Task<ActionResult> ChangeBillingAddress(ChangeBillingAddressModel model, CancellationToken cancellation)
        {
            if (!this.ModelState.IsValid) return this.View("ChangeBillingAddress", model);

            try
            {
                var client = await this.context
                .Set<ChargePayment>()
                .Where(c => c.Id == model.CardId)
                .Select(c => new { c.Client.UserId, CardId = c.Profile.PublicKey })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellation);
                if (client == null) return this.DisplayErrorResult($"An error has occurred while updating the card. The card {model.CardId} does not exist.");

                var address = new BillingAddressPayload();

                address.BusinessName = model.Address.BusinessName;
                address.Address = model.Address.Address;
                address.City = model.Address.City;
                address.State = model.Address.State;
                address.PostalCode = model.Address.Zip;
                address.Country = model.Address.Country;
                address.FirstName = model.Address.FirstName;
                address.LastName = model.Address.LastName;
                address.PhoneNumber = model.Address.PhoneNumber;

                var command = new UpdateBillingAddressCommand();
                command.CardId = client.CardId;
                command.BillingAddress = address;

                await this.bus.SendLocal(command);

                // Fake wait processing time because we're not using callbacks
                await Task.Delay(TimeSpan.FromSeconds(4), cancellation);

                return this.NavigationFor<ViewCreditCardsController>().Detail(client.UserId);

            }
            catch (Exception ex)
            {
                Logger.LogEvent(ex, Severity.High, $"Error updating card: {model.CardId}");

                return this.DisplayErrorResult($"An error has occurred while updating the card. Message: {ex.Message}");
            }
        }

        #endregion
    }
}
