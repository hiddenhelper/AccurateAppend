using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.ChargeProcessing.Contracts;
using AccurateAppend.Sales;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.Websites.Clients.Areas.Shared.Models;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas
{
    public abstract class OrderControllerBase : Controller
    {
        protected virtual async Task<Boolean> RequiresNewPaymentInformation(ISessionContext context, CancellationToken cancellation)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            var accounts = await context.SetOf<CreditCardRef>()
                .CardsForInteractiveUserAsync()
                .AsNoTracking()
                .OrderByDescending(c => c.IsPrimary)
                .ThenByDescending(c => c.Id)
                .ToArrayAsync(cancellation)
                .ConfigureAwait(false);

            if (!accounts.Any()) return true;

            return !accounts.Any(c => c.IsValid());
        }

        protected virtual Task UpdatePayment(ISessionContext context, IMessageSession bus, PaymentDetailsModel paymentModel, CancellationToken cancellation)
        {
            var address = new BillingAddressPayload();
            address.FirstName = paymentModel.CardHolderFirstName;
            address.LastName = paymentModel.CardHolderLastName;
            address.BusinessName = paymentModel.CardHolderBusinessName;
            address.PostalCode = paymentModel.CardPostalCode;
            address.PhoneNumber = paymentModel.CardHolderPhone;

            var card = new CreditCardPayload(this.EncryptPayload(paymentModel.CardNumber), paymentModel.GetExpirationDate(), paymentModel.CardCvv);

            var command = new CreatePaymentProfileCommand
            {
                UserId = this.User.Identity.GetIdentifier(),
                Card = card,
                BillingAddress = address,
                RequestId = Guid.NewGuid() // Don't care
            };

            return bus.Send(command);
        }

        protected abstract String EncryptPayload(String secureThisValue);

        protected virtual async Task<ActionResult> ForcePaymentScreenIfRequired(ISessionContext context, NewOrderModel orderModel, PaymentDetailsModel paymentModel, CancellationToken cancellation)
        {
            OrderPaymentViewPresenter model = null;

            // uncomment this line to test the payment form
            //return this.View("OrderPaymentForm",  this.CreatePaymentViewPresenter(orderModel, paymentModel));

            #region Valid Payment?

            // If no current valid payment method, display form
            if (paymentModel == null && await this.RequiresNewPaymentInformation(context, cancellation))
            {
                this.ModelState.Clear(); // hack to prevent error display on first render
                model = this.CreatePaymentViewPresenter(orderModel, new PaymentDetailsModel());
            }

            #endregion

            #region New Payment But Invalid?

            // New payment method supplied but not valid, display form
            if (paymentModel != null && !this.TryValidateModel(paymentModel))
            {
                model = this.CreatePaymentViewPresenter(orderModel, paymentModel);
            }

            #endregion

            if (model == null) return null;

            return this.View("OrderPaymentForm", model);
        }

        protected abstract OrderPaymentViewPresenter CreatePaymentViewPresenter(NewOrderModel orderModel, PaymentDetailsModel paymentModel);
    }
}