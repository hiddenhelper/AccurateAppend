using System.Threading.Tasks;
using System.Transactions;
using AccurateAppend.ChargeProcessing.Contracts;
using Microsoft.AspNet.SignalR;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Profile.Card.Messaging
{
    /// <summary>
    /// Responds to the <see cref="PaymentProfileCreatedEvent"/> by indicating to the original client
    /// requesting the operation the operation is completed via the SignalR backplane.
    /// </summary>
    /// <remarks>
    /// This handler is designed to operate outside of any ambient transactions as speed is paramount.
    /// </remarks>
    public class CallbackForPaymentProfileCreatedEventHandler : IHandleMessages<PaymentProfileCreatedEvent>
    {
        #region IHandleMessages<PaymentProfileCreatedEvent> Members

        /// <inheritdoc />
        public virtual Task Handle(PaymentProfileCreatedEvent message, IMessageHandlerContext context)
        {
            var connectionId = message.RequestId;

            using (var transaction = new TransactionScope(TransactionScopeOption.Suppress))
            {
                var callback = GlobalHost.ConnectionManager.GetHubContext<CallbackHub>();
                // uncomment this to eventually test connect callback issues (if any)
                //callback.Clients.Group(connectionId.ToString()).callbackComplete();

                callback.Clients.Client(connectionId.ToString()).callbackComplete();

                transaction.Complete();
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}