using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Messages;
using AccurateAppend.Sales.Contracts.ViewModels;
using AccurateAppend.Sales.DataAccess;
using NServiceBus;

namespace DomainModel.Services
{
    /// <summary>
    /// High level API for refund order process management.
    /// </summary>
    /// <remarks>
    /// Keeps the application service layer free of the rare (hopefully =D) refund process logic.
    /// </remarks>
    public class DealRefundService
    {
        #region Fields

        private readonly DefaultContext context;
        private readonly IMessageSession bus;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DealRefundService"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> used to provide data access.</param>
        /// <param name="bus">The <see cref="IMessageSession"/> used to send messages.</param>
        public DealRefundService(DefaultContext context, IMessageSession bus)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            Contract.EndContractBlock();

            this.context = context;
            this.bus = bus;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Acquires an open <see cref="RefundOrder"/> or creates a new one for the indicated deal.
        /// </summary>
        /// <param name="dealId">The identifier of the deal to open the <see cref="RefundOrder"/> for.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        public virtual async Task<Int32> CreateRefund(Int32 dealId, CancellationToken cancellation = default(CancellationToken))
        {
            var deal = await this.context
                .SetOf<DealBinder>()
                .CanBeRefunded()
                .Include(d => d.Orders)
                .FirstOrDefaultAsync(d => d.Id == dealId, cancellation)
                .ConfigureAwait(false);

            switch (deal)
            {
                case null:
                    throw new InvalidOperationException($"Deal {dealId} does not exist or cannot be refunded.");
                case LedgerDeal _:
                    throw new InvalidOperationException("Deal is a ledger deal from automated billing. This deal cannot be refunded via the admin. Please call support");
            }

            // Create refund
            var mutableDeal = deal as MutableDeal;
            var refundOrder = mutableDeal.CreateRefund();

            // New refund?
            if (refundOrder.Id == null)
            {
                // Create Audit
                var adminUser = Thread.CurrentPrincipal.Identity.GetIdentifier();
                deal.Notes.Add(new Audit("Refund created", adminUser));
            }

            // Get the original order and check for adjustments
            var originalOrder = refundOrder.OriginatingOrder();
            if (originalOrder.OrderMinimum > 0m)
            {
                var product = await this.context.SetOf<Product>().ForRefund().FirstAsync(cancellation);
                refundOrder.CreateLine(product).Price = (originalOrder.Total() - originalOrder.SubTotal()) * -1;
            }
            else if (originalOrder.OrderMinimum < 0m)
            {
                var product = await this.context.SetOf<Product>().ForAdjustToMinimum().FirstAsync(cancellation);
                refundOrder.CreateLine(product).Price = (originalOrder.Total() - originalOrder.SubTotal()) * -1;
            }

            await this.context.SaveChangesAsync(cancellation);

            return refundOrder.Id.Value;
        }

        private void T()
        {

        }
        /// <summary>
        /// Locates the indicated editable refund model by identifier, if exists.
        /// </summary>
        /// <param name="orderId">The identifier of the editable refund to find.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        /// <returns>If found and editable, the <see cref="RefundDetail"/> information; Otherwise null.</returns>
        public async Task<RefundDetail> FindRefund(Int32 orderId, CancellationToken cancellation = default(CancellationToken))
        {
            var query = this.context
                .SetOf<RefundOrder>()
                .AreEditable()
                .Where(o => o.Id == orderId);

            var order = await this.FindOrder(query, cancellation).ConfigureAwait(false);
            return order;
        }

        /// <summary>
        /// Stores the updated <see cref="RefundDetail"/> content.
        /// </summary>
        /// <param name="model">The new content to be updated.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        public async Task Update(RefundDetail model, CancellationToken cancellation = default(CancellationToken))
        {
            Validator.ValidateObject(model, new ValidationContext(model, null, null));

            var orderId = model.Id;
            var userId = model.UserId;

            var entity = await this.context
                .SetOf<RefundOrder>()
                .AreEditable()
                .Include(o => o.Lines)
                .Include(o => o.Deal)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.Deal.Client.UserId == userId, cancellation)
                .ConfigureAwait(false);

            if (entity == null) throw new InvalidOperationException("Order does not exist or cannot be edited.");

            var originDeal = entity.Deal;

            // Preload Products
            {
                var productNames = model.Items.Select(i => i.ProductName).ToArray();
                await System.Data.Entity.QueryableExtensions.LoadAsync(this.context
                    .SetOf<Product>()
                    .Where(p => productNames.Contains(p.Key)), cancellation);
            }

            foreach (var orderItem in model.Items.Where(i => i.Quantity > 0))
            {
                var origionalProduct = entity.Lines.FirstOrDefault(p => p.Product.Equals(orderItem.ProductName));

                if (origionalProduct == null) throw new ValidationException($"The input values contained a refund for {orderItem.ProductName} that was not found on the original order");

                if (origionalProduct.Maximum < orderItem.Quantity)
                {
                    //TODO: wire up validation. For now, boot to error screen
                    throw new ValidationException($"Product {origionalProduct.Product} can only refund up to {origionalProduct.Quantity} item(s) from the original order");
                }

                // Set quantity
                origionalProduct.Quantity = orderItem.Quantity;
            }

            // Confirm refund total doesn't exceed the remaining deal amount
            if (originDeal.Total() < 0m) throw new InvalidOperationException($"Refund order {entity.Id} exceeds the remaining amount on the original deal");

            await this.context.SaveChangesAsync(cancellation);
        }

        /// <summary>
        /// Performs the deal refund step.
        /// </summary>
        /// <remarks>
        /// Publishes the <see cref="DealRefundedEvent"/> event.
        /// </remarks>
        /// <param name="model">The new content to be updated.</param>
        /// <param name="cancellation">The <see cref="CancellationToken"/> used to signal the desire for an asynchronous process to cancel.</param>
        public virtual async Task Draft(RefundViewModel model, CancellationToken cancellation = default(CancellationToken))
        {
            Validator.ValidateObject(model, new ValidationContext(model, null, null));

            var order = await this.context
                .SetOf<DealBinder>()
                .CanBeRefunded()
                .Where(d => d.Id == model.Id)
                .SelectMany(d => d.Orders)
                .Where(o => o.Status == OrderStatus.Open)
                .OfType<RefundOrder>()
                .Include(o => o.Deal)
                .Include(o => o.Deal.Client)
                .FirstOrDefaultAsync(cancellation)
                .ConfigureAwait(false);

            if (order == null) throw new InvalidOperationException("The deal does not exist or is not eligible to be refunded.");

            var communication = order.Content ?? new BillContent(model.SendFrom);
            communication.Subject = model.Subject;
            communication.IsHtml = model.IsHtml;
            communication.Body = model.Body;

            foreach (var recipient in model.SendTo.Select(s => s.Trim()))
            {
                communication.SendTo.Add(new MailAddress(recipient));
            }
            foreach (var recipient in model.BccTo.Select(s => s.Trim()))
            {
                communication.BccTo.Add(new MailAddress(recipient));
            }

            order.DraftRefund(communication);

            var command = new AccurateAppend.ChargeProcessing.Contracts.ProcessRefundCommand();
            command.OrderId = order.Id.Value;
            command.Amount = order.Total() * -1;
            command.PublicKey = new Guid(order.PublicKey);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                await this.context.SaveChangesAsync(cancellation);
                await this.bus.SendLocal(command);

                transaction.Complete();
            }
        }

        #endregion

        private async Task<RefundDetail> FindOrder(IQueryable<RefundOrder> query, CancellationToken cancellation)
        {
            var order = await query.Select(o =>
                    new
                    {
                        o.Id,
                        o.OrderMinimum,
                        o.PublicKey,
                        o.Deal.Client.UserId,
                        Lines = o.Lines.Select(l =>
                            new
                            {
                                l.Id,
                                l.Price,
                                l.Description,
                                l.Maximum,
                                ProductName = l.Product.Key,
                                l.Quantity
                            }
                        )
                    }
                )
                .FirstOrDefaultAsync(cancellation)
                .ConfigureAwait(false);
            if (order == null) return null;

            var model = new RefundDetail()
            {
                Id = order.Id.Value,
                OrderMinimum = order.OrderMinimum,
                PublicKey = order.PublicKey,
                UserId = order.UserId
            };

            model.Items.AddRange(
                order.Lines.Select(line =>

                    new OrderItemModel()
                    {
                        Id = line.Id,
                        Cost = line.Price,
                        Description = line.Description,
                        Maximum = line.Maximum,
                        ProductName = line.ProductName,
                        Quantity = line.Quantity,
                        UserId = order.UserId
                    }));

            return model;
        }
    }
}
