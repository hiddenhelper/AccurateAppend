#pragma warning disable SA1649 // File name must match first type name

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using AccurateAppend.Core.Definitions;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Extension methods for the <seealso cref="Product"/> type.
    /// </summary>
    public static class ProductExtensions
    {
        /// <summary>
        /// Provides a readonly collection of <see cref="Product.Key"/> values that are considered to be
        /// products that limit quantity and cost to the original order during refunds.
        /// </summary>
        /// <remarks>
        /// Product Operation match refund items are ultimately limited to the content of the original order.
        /// This means that if PHONE_PREM had 400 items at .01 PPU, the total number of PHONE_PREM refund items
        /// in a deal can only total up to 400. In addition, the refund price is fixed. Other types of products
        /// such as a Subscription prepayment line or Custom Development may instead have a single item in the
        /// original order. Refunds for those products can have arbitrary PPU on the refunds provided the entire
        /// total refunds for the product itself do not exceed the original billed total.
        /// </remarks>
        public static readonly IList<String> RestrictedRefundProductKeys;

        /// <summary>
        /// Provides a readonly collection of <see cref="Product.Key"/> values that are considered to be
        /// non-billable products.
        /// </summary>
        public static readonly IList<String> NonBillableProductKeys;

        /// <summary>
        /// Type initializer.
        /// </summary>
        static ProductExtensions()
        {
            var pref = DataServiceOperationExtensions.PreferenceOperations.Select(o => o.ToString()).ToArray();
            NonBillableProductKeys = new ReadOnlyCollection<String>(pref);

            var keys = Enum.GetNames(typeof(DataServiceOperation));
            RestrictedRefundProductKeys = new ReadOnlyCollection<String>(keys);
        }

        /// <summary>
        /// Crafts a query predicate that can filter the <seealso cref="Product"/> queryable to contain
        /// only billable products.
        /// </summary>
        /// <remarks>
        /// <note type="Warning">
        /// The predicate filter is added to the provided <paramref name="queryable"/> instance. 
        /// It does not guarantee that any <see cref="Product"/> instances will be returned if the originating
        /// predicate produces no results. 
        /// </note>
        /// </remarks>
        /// <param name="queryable">The source query to acquire the data from.</param>
        /// <returns>A queryable that can return only billable <seealso cref="Product"/> instances.</returns>
        public static IQueryable<Product> FilterNonBillableProducts(this IQueryable<Product> queryable)
        {
            if (queryable == null) throw new ArgumentNullException(nameof(queryable));

            Contract.Ensures(Contract.Result<IQueryable<Product>>() != null);
            Contract.EndContractBlock();

            return queryable.Where(p => !NonBillableProductKeys.Contains(p.Key));
        }
    }
}