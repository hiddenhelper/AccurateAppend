using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccurateAppend.ChargeProcessing.Contracts;
using AccurateAppend.Data;
using AccurateAppend.Messaging;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Messages;
using NServiceBus;
using TransactionResult = AccurateAppend.Core.Definitions.TransactionResult;

namespace AccurateAppend.Websites.Admin.Areas.Sales.SubmitCharge.Messaging
{
    /// <summary>
    /// Handler for the <see cref="ChargeProcessedEvent"/> bus message.
    /// </summary>
    /// <remarks>
    /// Responds to a message by posting a <see cref="TransactionEvent"/>
    /// to an existing <see cref="BillableOrder"/>.
    /// </remarks>
    public class ChargeProcessedEventHandler : IHandleMessages<ChargeProcessedEvent>
    {
        #region Fields

        private readonly AccurateAppend.Sales.DataAccess.DefaultContext dbContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChargeProcessedEventHandler"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="AccurateAppend.Sales.DataAccess.DefaultContext"/> dal component.</param>
        public ChargeProcessedEventHandler(AccurateAppend.Sales.DataAccess.DefaultContext dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            this.dbContext = dbContext;
        }

        #endregion

        #region IHandleMessages<ChargeProcessedEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(ChargeProcessedEvent message, IMessageHandlerContext context)
        {
            var orderId = message.OrderId;
            var publicKey = message.PublicKey;
            var amountCharged = message.AmountCharged;
            var amountProcessed = message.AmountProcessed;
            
            var processedBy = context.InitiatingUserId();
            
            var order = await this.dbContext
                .SetOf<BillableOrder>()
                .Where(o => o.Id == orderId)
                .Include(o => o.Deal)
                .Include(o => o.Transactions)
                .Include(o => o.PendingTransactions)
                .Include(o => o.Lines)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            if (order == null) throw new InvalidOperationException($"Order {orderId} not found");

            if (order.Transactions.Any(e => e.PublicKey == publicKey)) return;
            var request = order.PendingTransactions.First(t => t.PublicKey == publicKey);

            var transactions = new List<TransactionEvent>();

            foreach (var processing in message.Processing.OrderBy(e => e.EventDate))
            {
                var status = (TransactionResult) (Int32) processing.Status;
                var eventDate = processing.EventDate;
                var transactionId = processing.TransactionId;
                var skipAvs = processing.AvsSkipped;
                var description = processing.Description;

                // Audit - by putting this block first we guarantee deal is loaded
                {
                    if (skipAvs) order.Deal.Notes.Add(new Audit($"AVS skipped charging Order: {order.Id}", processedBy, eventDate));

                    Audit audit;
                    if (status.IsCaptured() && status == TransactionResult.Approved)
                    {
                        audit = new Audit($"Charge processed: {transactionId}", processedBy, eventDate);
                    }
                    else if (status.IsCaptured() && status == TransactionResult.Voided)
                    {
                        audit = new Audit($"Voided: {description}", processedBy, eventDate);
                    }
                    else
                    {
                        audit = new Audit($"Charge not processed: {description}", processedBy, eventDate);
                    }

                    order.Deal.Notes.Add(audit);
                }

                var transaction = request.Complete(status, amountCharged, amountProcessed);
                transactions.Add(transaction);
            }

            order.PostCharge(transactions.ToArray());

            await this.dbContext.SaveChangesAsync();

            if (order.Deal.Status == DealStatus.Complete)
            {
                var deal = order.Deal;

                var @event = new DealCompletedEvent
                {
                    Client = deal.Client.UserId,
                    Amount = deal.Amount,
                    DealId = deal.Id.Value,
                    PublicKey = new Guid(deal.Orders.First().PublicKey)
                };

                await context.Publish(@event);
            }
        }

        #endregion
    }
}