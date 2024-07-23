#pragma warning disable SA1649 // File name must match first type name

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Extension methods for the <see cref="ProductLine"/> entity.
    /// </summary>
    public static class ProductLineExtensions
    {
        /// <summary>
        /// Filters the order set to contain only items that are considered billable products.
        /// </summary>
        /// <param name="lines">The collection of <see cref="ProductLine"/> instances to search.</param>
        public static IEnumerable<ProductLine> FilterNonBillableOperations(this ICollection<ProductLine> lines)
        {
            if (lines == null) throw new ArgumentNullException(nameof(lines));
            Contract.Ensures(Contract.Result<IEnumerable<ProductLine>>() != null);
            Contract.EndContractBlock();

            return lines.Where(i => i.IsBillable());
        }

        /// <summary>
        /// Creates or returns a limited refund <see cref="ProductLine"/> based on an existing instance.
        /// </summary>
        /// <returns>
        /// Will locate the first existing <see cref="ProductLine"/> with a matching <see cref="Product"/> or create one.
        /// </returns>
        /// <param name="orderLine">The original <see cref="ProductLine"/> to provide a refund to.</param>
        /// <param name="refundOrder">The <see cref="RefundOrder"/> to add the refund item to.</param>
        /// <param name="quantity">The number of units in the item to create.</param>
        /// <returns>The <see cref="ProductLine"/> attached to the <paramref name="refundOrder"/>.</returns>
        public static ProductLine CreateRefundItem(this ProductLine orderLine, RefundOrder refundOrder, Int32 quantity)
        {
            if (orderLine == null) throw new ArgumentNullException(nameof(orderLine));
            if (refundOrder == null) throw new ArgumentNullException(nameof(refundOrder));
            if (orderLine.Quantity < 1 || orderLine.Price <= 0) throw new ArgumentException($"The provided order line {orderLine.Id} has a quantity of {orderLine.Quantity} and price of {orderLine.Price} and cannot be refunded", nameof(orderLine));
            if (quantity > orderLine.Quantity) throw new ArgumentOutOfRangeException(nameof(quantity), quantity, $"Refund quantity cannot exceed original order line quantity of {orderLine.Quantity}");
            Contract.Ensures(Contract.Result<ProductLine>() != null);
            Contract.Ensures(refundOrder.Lines.Contains(Contract.Result<ProductLine>()));
            Contract.EndContractBlock();

            if (orderLine.Order is RefundOrder) throw new InvalidOperationException($"The order line {orderLine.Id} belongs to an refund order and cannot itself be refunded");
            if (!(orderLine.Total() > 0)) throw new InvalidOperationException($"The order line {orderLine.Id} has a total of {orderLine.Total()} and cannot be refunded");

            var item = refundOrder.Lines.FirstOrDefault(i => i.Product.Equals(orderLine.Product))
                       ??
                       refundOrder.CreateLine(orderLine.Product, quantity);
            item.Price = Math.Abs(orderLine.Price) * -1;

            return item;
        }

        /// <summary>
        /// Creates or returns an unlimited refund <see cref="ProductLine"/> based on an existing instance.
        /// </summary>
        /// <returns>
        /// Will locate the first existing <see cref="ProductLine"/> with a matching <see cref="Product"/> or create one.
        /// </returns>
        /// <param name="orderLine">The original <see cref="ProductLine"/> to provide a refund to.</param>
        /// <param name="refundOrder">The <see cref="RefundOrder"/> to add the refund item to.</param>
        /// <returns>The <see cref="ProductLine"/> attached to the <paramref name="refundOrder"/>.</returns>
        public static ProductLine CreateRefundItem(this ProductLine orderLine, RefundOrder refundOrder)
        {
            if (orderLine == null) throw new ArgumentNullException(nameof(orderLine));
            if (refundOrder == null) throw new ArgumentNullException(nameof(refundOrder));
            if (orderLine.Quantity < 1 || orderLine.Price <= 0) throw new ArgumentException($"The provided order line {orderLine.Id} has a quantity of {orderLine.Quantity} and price of {orderLine.Price} and cannot be refunded", nameof(orderLine));
            Contract.Ensures(Contract.Result<ProductLine>() != null);
            Contract.Ensures(refundOrder.Lines.Contains(Contract.Result<ProductLine>()));
            Contract.EndContractBlock();

            if (orderLine.Order is RefundOrder) throw new InvalidOperationException($"The order line {orderLine.Id} belongs to an refund order and cannot itself be refunded");
            if (!(orderLine.Total() > 0)) throw new InvalidOperationException($"The order line {orderLine.Id} has a total of {orderLine.Total()} and cannot be refunded");
            if (orderLine.HasRestrictedRefund()) throw new InvalidOperationException($"The order line {orderLine.Id} has product {orderLine.Product.Key} which is required to have restricted maximum refunds");

            var item = refundOrder.Lines.FirstOrDefault(i => i.Product.Equals(orderLine.Product))
                       ??
                       refundOrder.CreateLine(orderLine.Product);
            item.Price = Math.Abs(orderLine.Price) * -1;

            return item;
        }

        /// <summary>
        /// Gets the calculated total for all the supplied product items.
        /// </summary>
        /// <remarks>
        /// Within the context of an <see cref="Order"/>, this is effectively the sub-total value
        /// in that is the sum of the <see cref="ProductLine"/> without concern for <see cref="Order.OrderMinimum"/>
        /// concepts.
        /// </remarks>
        /// <returns>The calculated total for all the provided order items.</returns>
        public static Decimal Total(this IEnumerable<ProductLine> lines)
        {
            if (lines == null) throw new ArgumentNullException(nameof(lines));

            return lines.Where(l => l != null).Distinct().Sum(i => i.Total());
        }
    }
}

