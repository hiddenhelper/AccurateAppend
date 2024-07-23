using System;
using System.Linq;
using System.Threading.Tasks;
using AccurateAppend.ChargeProcessing.Contracts;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Messages;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Areas.Sales.RefundDeal.Messaging
{
    /// <summary>
    /// Handler for the <see cref="RefundProcessedEvent"/> bus message.
    /// </summary>
    /// <remarks>
    /// Responds to a message by posting to an existing <see cref="RefundOrder"/>.
    /// </remarks>
    public class RefundProcessedEventhandler : IHandleMessages<RefundProcessedEvent>
    {
        #region Fields

        private readonly AccurateAppend.Sales.DataAccess.DefaultContext dbContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RefundProcessedEventhandler"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="AccurateAppend.Sales.DataAccess.DefaultContext"/> dal component.</param>
        public RefundProcessedEventhandler(AccurateAppend.Sales.DataAccess.DefaultContext dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            this.dbContext = dbContext;
        }

        #endregion

        #region IHandleMessages<RefundProcessedEvent> Members

        /// <inheritdoc />
        public async Task Handle(RefundProcessedEvent message, IMessageHandlerContext context)
        {
            var orderId = message.OrderId;
            var publicKey = message.PublicKey;
            var status = (Core.Definitions.TransactionResult) (Int32) message.Status;
            var amountRefunded = message.AmountRefunded * -1;

            var order = await this.dbContext
                .SetOf<RefundOrder>()
                .Include(o => o.Deal)
                .Include(o => o.Deal.Client)
                .Where(o => o.Id == orderId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            if (order == null) throw new InvalidOperationException($"Order {orderId} not found");

            if (order.Transactions.Any(e => e.PublicKey == publicKey)) return;

            var chargeEvent = new TransactionEvent(order, status, publicKey, amountRefunded);
            order.PostRefund(chargeEvent);

            await this.dbContext.SaveChangesAsync();

            var @event = new DealRefundedEvent
            {
                Client = order.Deal.Client.UserId,
                Amount = amountRefunded,
                DealId = order.Deal.Id.Value,
                PublicKey = publicKey
            };

            await context.Publish(@event);
        }

        #endregion
    }
}