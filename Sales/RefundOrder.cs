using System;
using System.Diagnostics.Contracts;
using System.Linq;
using AccurateAppend.Core.Definitions;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// A specialized <see cref="Order"/> that can be added to a completed <see cref="DealBinder"/> and used to track refunds.
    /// </summary>
    public class RefundOrder : Order
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RefundOrder"/> class.
        /// </summary>
        /// <remarks>Used in ORM, Serialization, and subclassing scenarios.</remarks>
        protected RefundOrder()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RefundOrder"/> class.
        /// </summary>
        /// <remarks>This instance will automatically be added to the <see cref="DealBinder.Orders"/> collection.</remarks>
        /// <param name="deal">The <see cref="DealBinder"/> that contains this order instance.</param>
        public RefundOrder(DealBinder deal) : base(deal, Guid.NewGuid())
        {
            if (deal.Status != DealStatus.Complete) throw new InvalidOperationException($"Deal {deal.Id} must be complete before refunds may be issued");
            Contract.EndContractBlock();

            if (deal.Orders.OfType<RefundOrder>()
                .Where(o => !o.Equals(this))
                .Any(o => o.Status == OrderStatus.Open))
            {
                deal.Orders.Remove(this);
                throw new InvalidOperationException($"Deal {deal.Id} already has an open Refund order. A new one cannot be created");
            }

            var billedProducts = deal.Orders.OfType<BillableOrder>().Where(o => o.Status == OrderStatus.Billed).SelectMany(o => o.Lines).Where(i => i.Total() > 0);
            var refundedProducts = deal.Orders.OfType<RefundOrder>().Where(o => o.Status == OrderStatus.Refunded).SelectMany(o => o.Lines).ToArray();

            foreach (var grouping in billedProducts.GroupBy(l => new { l.Product.Key, l.Price, IsRestricted = l.HasRestrictedRefund()}))
            {
                if (grouping.Key.IsRestricted)
                {
                    // Check for how many possible items we can use on this refund and if greater than 0, create a limited refund
                    var quantityBilled = grouping.Sum(g => g.Quantity);
                    var quantityRefunded = refundedProducts.Where(i => grouping.Key.Key == i.Product.Key).Sum(i => i.Quantity);
                    var quantityAvailable = quantityBilled - quantityRefunded;
                    if (quantityAvailable < 1) continue;

                    grouping.First().CreateRefundItem(this, quantityAvailable);
                }
                else
                {
                    // Create an unbound refund
                    grouping.First().CreateRefundItem(this);
                }
            }

            // set adjustments
            //var adjustment = deal.Orders.OfType<BillableOrder>().Sum(o => o.OrderMinimum);
            //adjustment = adjustment - (deal.Orders.OfType<RefundOrder>().Sum(o => o.Adjustment) * -1);

            //base.Adjustment = adjustment;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Gets the calculated total for all the current order items.
        /// </summary>
        /// <remarks>
        /// This value is affected by the presence of a non zero value in the
        /// <see cref="Order.OrderMinimum"/> property. If <seealso cref="Order.OrderMinimum"/> is greater than
        /// zero and greater than calculated total, the minimum adjusted value will be returned
        /// instead. If <seealso cref="Order.OrderMinimum"/> is a negative value then the total will
        /// be the difference of the values or 0, whichever is greater.
        /// </remarks>
        /// <returns>The calculated total for all the current order items.</returns>
        public override Decimal Total()
        {
            var subTotal = this.Lines.Sum(i => i.Total());
            if (this.OrderMinimum == 0) return subTotal;

            if (this.OrderMinimum > 0)
            {
                subTotal = subTotal + this.OrderMinimum;
                subTotal = Math.Min(subTotal, 0);
            }
            else
            {
                subTotal = Math.Min(subTotal, this.OrderMinimum);
            }

            return subTotal;
        }
        
        ///// <summary>
        ///// Called whenever a new <see cref="OrderLine"/> is added or when an existing instance is changed.
        ///// </summary>
        ///// <param name="orderLine">The <see cref="OrderLine"/> instance that is new or changed.</param>
        //protected internal override void Validate(OrderLine orderLine)
        //{
        //    if (!this.IsInitialized) return;

        //    base.Validate(orderLine);

        //    if (orderLine.Total() > 0) throw new InvalidOperationException("A RefundOrder may not contain order lines that have a positive total.");
        //    if (orderLine.Maximum == null)
        //    {
        //        throw new InvalidOperationException("A RefundOrder may not contain order lines that lack a maximum quantity amount.");
        //    }

        //    if (orderLine.Maximum < orderLine.Quantity)
        //    {
        //        throw new InvalidOperationException($"A RefundOrder may not contain order lines that exceed the maximum quantity amount. Max={orderLine.Maximum}, Quantity={orderLine.Quantity}");
        //    }
        //}

        ///// <summary>
        ///// Configures the current instance to contain a bill content that is externally created.
        ///// </summary>
        ///// <param name="billType">The <see cref="BillType"/> to draft.</param>
        ///// <param name="bill">The <see cref="Message"/> containing the bill information to send for the current order.</param>
        ///// <returns>The <see cref="Order.Bill"/> for the current order.</returns>
        //public override void DraftBill(BillType billType, Message bill)
        //{
        //    if (billType == BillType.Invoice) throw new NotSupportedException("Invoice refunds are not supported");

        //    base.DraftBill(billType, bill);
        //}

        #endregion

        #region Methods

        /// <summary>
        /// Posts the provided <paramref name="refund"/> to the current instance. The order
        /// will be Refunded and the <see cref="Order.Complete"/> event is raised.
        /// </summary>
        /// <param name="refund">The <see cref="TransactionEvent"/> to post to this order.</param>
        public virtual void PostRefund(TransactionEvent refund)
        {
            if (this.Status != OrderStatus.Open) throw new InvalidOperationException($"{nameof(RefundOrder)} is not in the {OrderStatus.Open} status");
            if (refund == null) throw new ArgumentNullException(nameof(refund));
            if (!refund.Order.Equals(this)) throw new ArgumentOutOfRangeException(nameof(refund), refund.Order.PublicKey, $"{nameof(TransactionEvent)} {refund.PublicKey} does not belong to {nameof(RefundOrder)} {this.PublicKey}");
            Contract.EndContractBlock();

            this.Transactions.Add(refund);

            if (this.Total() <= 0)
            {
                if (refund.Status == TransactionResult.Refunded) this.OnComplete(OrderStatus.Refunded);
            }
        }

        /// <summary>
        /// Accesses the original <see cref="BillableOrder"/> this refund is for.
        /// </summary>
        /// <remarks>
        /// Is really just a shortcut to running the <see cref="DealBinderExtensions.OriginatingOrder"/> query.
        /// </remarks>
        /// <returns>The original <see cref="BillableOrder"/>.</returns>
        public virtual BillableOrder OriginatingOrder()
        {
            Contract.Ensures(Contract.Result<BillableOrder>() != null);
            Contract.EndContractBlock();

            var originalOrder = this.Deal.OriginatingOrder();
            return originalOrder;
        }

        #endregion
    }
}
