#pragma warning disable SA1649 // File name must match first type name
using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;

namespace AccurateAppend.Sales.DataAccess
{
    /// <summary>
    /// Provides extension methods for the <see cref="Product"/> entity.
    /// </summary>
    public static class ProductQueryExtensions
    {
        /// <summary>
        /// Crafts a query predicate that can acquire the <see cref="Product"/> instance for the "Adjust To Minimum" product.
        /// </summary>
        /// <remarks>
        /// <note type="Warning">
        /// The predicate filter is added to the provided <paramref name="queryable"/> instance. 
        /// It does not guarantee that a <see cref="Product"/> instance will be returned if the originating
        /// predicate produces no results. 
        /// </note>
        /// </remarks>
        /// <param name="queryable">The source query to acquire the data from.</param>
        /// <returns>A queryable that can return the requested product.</returns>
        public static IQueryable<Product> ForAdjustToMinimum(this IQueryable<Product> queryable)
        {
            if (queryable == null) throw new ArgumentNullException(nameof(queryable));
            Contract.Ensures(Contract.Result<IQueryable<Product>>() != null);
            Contract.EndContractBlock();

            return queryable.Where(p => p.Key == "AdjustToMinimum");
        }

        /// <summary>
        /// Crafts a query predicate that can acquire the <see cref="Product"/> instance for the "Refund" product.
        /// </summary>
        /// <remarks>
        /// <note type="Warning">
        /// The predicate filter is added to the provided <paramref name="queryable"/> instance. 
        /// It does not guarantee that a <see cref="Product"/> instance will be returned if the originating
        /// predicate produces no results. 
        /// </note>
        /// </remarks>
        /// <param name="queryable">The source query to acquire the data from.</param>
        /// <returns>A queryable that can return the requested product.</returns>
        public static IQueryable<Product> ForRefund(this IQueryable<Product> queryable)
        {
            if (queryable == null) throw new ArgumentNullException(nameof(queryable));
            Contract.Ensures(Contract.Result<IQueryable<Product>>() != null);
            Contract.EndContractBlock();

            return queryable.Where(p => p.Key == "Refund");
        }

        /// <summary>
        /// Crafts a query predicate that can acquire the <see cref="Product"/> instance for the "Subscription" product.
        /// </summary>
        /// <remarks>
        /// <note type="Warning">
        /// The predicate filter is added to the provided <paramref name="queryable"/> instance. 
        /// It does not guarantee that a <see cref="Product"/> instance will be returned if the originating
        /// predicate produces no results. 
        /// </note>
        /// </remarks>
        /// <param name="queryable">The source query to acquire the data from.</param>
        /// <returns>A queryable that can return the requested product.</returns>
        public static IQueryable<Product> ForSubscription(this IQueryable<Product> queryable)
        {
            if (queryable == null) throw new ArgumentNullException(nameof(queryable));
            Contract.Ensures(Contract.Result<IQueryable<Product>>() != null);
            Contract.EndContractBlock();

            return queryable.Where(p => p.Key == "Subscription");
        }
    }
}