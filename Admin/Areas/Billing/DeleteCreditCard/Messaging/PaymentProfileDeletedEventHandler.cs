using System.Threading.Tasks;
using AccurateAppend.ChargeProcessing.Contracts;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Areas.Billing.DeleteCreditCard.Messaging
{
    public class PaymentProfileDeletedEventHandler : IHandleMessages<PaymentProfileDeletedEvent>
    {
        #region IHandleMessages<PaymentProfileDeletedEvent> members

        public virtual Task Handle(PaymentProfileDeletedEvent message, IMessageHandlerContext context)
        {
            return Task.CompletedTask;
        }

        #endregion
    }
}