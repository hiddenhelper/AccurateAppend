#pragma warning disable SA1649 // File name must match first type name

using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Provides extension methods for the <see cref="MutableDeal"/> type.
    /// </summary>
    public static class MutableDealExtensions
    {
        /// <summary>
        /// Convenience method to quickly create a new <see cref="BillableOrder"/> and append it to the provided <paramref name="deal"/> orders.
        /// </summary>
        /// <param name="deal">The <see cref="MutableDeal"/> to append an order to.</param>
        /// <returns>The new <see cref="BillableOrder"/> instance.</returns>
        public static BillableOrder CreateOrder(this MutableDeal deal)
        {
            return deal.CreateOrder(Guid.NewGuid());
        }

        /// <summary>
        /// Convenience method to quickly create a new <see cref="BillableOrder"/> and append it to the provided <paramref name="deal"/> orders.
        /// </summary>
        /// <param name="deal">The <see cref="MutableDeal"/> to append an order to.</param>
        /// <param name="publicKey">The public key uniquely identifying the <see cref="BillableOrder"/> to client file.</param>
        /// <returns>The new <see cref="BillableOrder"/> instance.</returns>
        public static BillableOrder CreateOrder(this MutableDeal deal, Guid publicKey)
        {
            if (deal == null) throw new ArgumentNullException(nameof(deal));
            Contract.Ensures(Contract.Result<BillableOrder>() != null);
            Contract.EndContractBlock();

            var order = new BillableOrder(deal, publicKey);
            return order;
        }

        /// <summary>
        /// Convenience method to quickly create a new <see cref="RefundOrder"/> or return the current one and append it to the provided <paramref name="deal"/> orders.
        /// </summary>
        /// <param name="deal">The <see cref="MutableDeal"/> to append a <see cref="RefundOrder"/> to.</param>
        /// <returns>The new <see cref="RefundOrder"/> instance.</returns>
        public static RefundOrder CreateRefund(this MutableDeal deal)
        {
            if (deal == null) throw new ArgumentNullException(nameof(deal));
            Contract.EndContractBlock();

            if (deal.Status != DealStatus.Complete) throw new ArgumentOutOfRangeException(nameof(deal), deal.Status, $"Deals must be {DealStatus.Complete} to issue a refund");
            var order = deal
                            .Orders
                            .OfType<RefundOrder>()
                            .FirstOrDefault(o => o.Status == OrderStatus.Open)
                        ??
                        new RefundOrder(deal);
            return order;
        }
    }
}
