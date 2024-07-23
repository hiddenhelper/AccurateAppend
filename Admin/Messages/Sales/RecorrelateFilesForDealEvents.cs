using System;
using System.Threading.Tasks;
using AccurateAppend.Messaging;
using AccurateAppend.Operations.Contracts;
using AccurateAppend.Sales.Contracts.Messages;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Messages.Sales
{
    /// <summary>
    /// Handler for the <see cref="DealPublicKeyChangedEvent"/> and <see cref="DealCanceledEvent"/> bus messages.
    /// </summary>
    /// <remarks>
    /// Responds to an event by issuing a <see cref="UpdateFileCorrelationIdCommand"/> for the PublicKey values.
    /// </remarks>
    public class RecorrelateFilesForDealEvents : IHandleMessages<DealPublicKeyChangedEvent>, IHandleMessages<DealCanceledEvent>
    {
        #region IHandleMessages<DealPublicKeyChangedEvent> Members

        /// <inheritdoc />
        public virtual Task Handle(DealPublicKeyChangedEvent message, IMessageHandlerContext context)
        {
            using (context.Alias())
            {
                // Since the PublicKey value has changed, update any correlated files as well
                var command = new UpdateFileCorrelationIdCommand
                {
                    RequestId = Guid.NewGuid(),
                    NewCorrelationId = message.NewPublicKey,
                    OriginalCorrelationId = message.OriginalPublicKey
                };

                return context.Send(command);
            }
        }

        #endregion

        #region IHandleMessages<DealCanceledEvent> Members

        /// <inheritdoc />
        public virtual Task Handle(DealCanceledEvent message, IMessageHandlerContext context)
        {
            using (context.Alias())
            {
                // Since the PublicKey value has changed, un-correlated files as well
                var command = new UpdateFileCorrelationIdCommand
                {
                    RequestId = Guid.NewGuid(),
                    NewCorrelationId = null,
                    OriginalCorrelationId = message.PublicKey
                };

                return context.Send(command);
            }
        }

        #endregion
    }
}