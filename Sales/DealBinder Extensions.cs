#pragma warning disable SA1649 // File name must match first type name

using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Extension methods for the <see cref="DealBinder"/> type.
    /// </summary>
    public static class DealBinderExtensions
    {
        /// <summary>
        /// Locates the first (and only) <see cref="BillableOrder"/> in a <see cref="DealBinder"/>.
        /// </summary>
        /// <remarks>
        /// We can safely assume this aS A <see cref="DealBinder"/> has EXACTLY 1 <seealso cref="BillableOrder"/>
        /// present always.
        /// </remarks>
        /// <returns>The one and only instance of the <see cref="BillableOrder"/> in the <paramref name="deal"/>.</returns>
        public static BillableOrder OriginatingOrder(this DealBinder deal)
        {
            if (deal == null) throw new ArgumentNullException(nameof(deal));
            Contract.Ensures(Contract.Result<BillableOrder>() != null);
            Contract.EndContractBlock();

            // We can safely run a First instead of FirstOrDefault as there's exactly 1
            return deal.Orders.OfType<BillableOrder>().First();
        }
    }
}
