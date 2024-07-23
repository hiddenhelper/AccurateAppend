#pragma warning disable SA1649 // File name must match first type name

using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Extension methods for the <see cref="Order"/> class.
    /// </summary>
    public static class OrderExtensions
    {
        internal static readonly OrderStatus[] EditableStatus = {OrderStatus.Open};

        internal static readonly OrderStatus[] CompletedStatus = { OrderStatus.Billed, OrderStatus.WriteOff, OrderStatus.Refunded };

        /// <summary>
        /// Indicates whether the current status is in an editable state.
        /// </summary>
        /// <remarks>
        /// Current logic dictates that Open status is editable.
        /// </remarks>
        /// <returns>True if the the order is editable, otherwise false.</returns>
        public static Boolean CanBeEdited(this OrderStatus status)
        {
            return EditableStatus.Contains(status);
        }

        /// <summary>
        /// Indicates whether the current status is considered "complete". That is an <see cref="Order"/>
        /// that cannot have additional modifications performed.
        /// </summary>
        /// <remarks>
        /// Current logic dictates that Billed, WriteOff, and Refunded status are complete.
        /// </remarks>
        /// <returns>True if the the order is editable, otherwise false.</returns>
        public static Boolean IsComplete(this OrderStatus status)
        {
            return CompletedStatus.Contains(status);
        }

        /// <summary>
        /// Convenience method to quickly create a new <see cref="ProductLine"/> and append it to the provided <paramref name="order"/> item.
        /// </summary>
        /// <param name="order">The <see cref="Order"/> to append an order line to.</param>
        /// <param name="product">The <see cref="Product"/> that the new line should use.</param>
        /// <param name="maximum">The <see cref="ProductLine.Maximum"/> quantity that can be created.</param>
        /// <returns>The new <see cref="ProductLine"/> instance.</returns>
        public static ProductLine CreateLine(this Order order, Product product, Int32? maximum = null)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            if (product == null) throw new ArgumentNullException(nameof(product));
            Contract.Ensures(Contract.Result<ProductLine>() != null);
            Contract.EndContractBlock();

            var line = new ProductLine(order, product, maximum);
            return line;
        }
    }
}
