#pragma warning disable SA1649 // File name must match first type name
using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;

namespace AccurateAppend.Sales.DataAccess
{
    /// <summary>
    /// Provides extension methods for the <see cref="Cart"/> entity.
    /// </summary>
    public static class CartQueryExtensions
    {
        #region Filters

        /// <summary>
        /// Crafts a query predicate that can acquire the <see cref="Cart"/> instance belonging to the current user.
        /// </summary>
        /// <remarks>
        /// <note type="Warning">
        /// The predicate filter is added to the provided <paramref name="queryable"/> instance. 
        /// It does not guarantee that a <see cref="Cart"/> instance will be returned if the originating
        /// predicate produces no results. 
        /// </note>
        /// </remarks>
        /// <param name="queryable">The source query to acquire the data from.</param>
        /// <returns>A queryable that can return carts for the current interactive user.</returns>
        public static IQueryable<Cart> ForInteractiveUser(this IQueryable<Cart> queryable)
        {
            if (queryable == null) throw new ArgumentNullException(nameof(queryable));
            Contract.Ensures(Contract.Result<IQueryable<Cart>>() != null);
            Contract.EndContractBlock();

            var username = Thread.CurrentPrincipal.Identity.Name;
            return queryable.Where(c => c.Client.UserName == username);
        }

        /// <summary>
        /// Crafts a query predicate that can acquire the <see cref="Cart"/> instance belonging to the indicated user.
        /// </summary>
        /// <remarks>
        /// <note type="Warning">
        /// The predicate filter is added to the provided <paramref name="queryable"/> instance. 
        /// It does not guarantee that a <see cref="Cart"/> instance will be returned if the originating
        /// predicate produces no results. 
        /// </note>
        /// </remarks>
        /// <param name="queryable">The source query to acquire the data from.</param>
        /// <param name="userId">The user identifier of the specific client.</param>
        /// <returns>A queryable that can return carts for the indicated user.</returns>
        public static IQueryable<Cart> ForClient(this IQueryable<Cart> queryable, Guid userId)
        {
            if (queryable == null) throw new ArgumentNullException(nameof(queryable));
            Contract.Ensures(Contract.Result<IQueryable<Cart>>() != null);
            Contract.EndContractBlock();

            return queryable.Where(c => c.Client.UserId == userId);
        }

        /// <summary>
        /// Crafts a query predicate that can acquire <see cref="CsvCart"/> instances that file analysis has been performed.
        /// </summary>
        /// <remarks>
        /// <note type="Warning">
        /// The predicate filter is added to the provided <paramref name="queryable"/> instance. 
        /// It does not guarantee that a <see cref="CsvCart"/> instance will be returned if the originating
        /// predicate produces no results.
        /// 
        /// In addition, this predicate does not limit filters to specific cart instances and therefore is best utilized
        /// with source queries that perform this logic. Allows callers to filter carts by the presence of an existing
        /// <see cref="CsvCart.Analysis"/> value on an otherwise unqueryable property.
        /// </note>
        /// </remarks>
        /// <param name="queryable">The source query to acquire the data from.</param>
        /// <returns>A new <see cref="IQueryable{CsvCart}"/> that can be further customized or used.</returns>
        public static IQueryable<CsvCart> WithCompletedAnalysis(this IQueryable<CsvCart> queryable)
        {
            if (queryable == null) throw new ArgumentNullException(nameof(queryable));
            Contract.Ensures(Contract.Result<IQueryable<Cart>>() != null);
            Contract.EndContractBlock();

            queryable = queryable.Where(c => c.AnalysisInternal != null);
            return queryable;
        }

        /// <summary>
        /// Crafts a query predicate that can acquire <see cref="Cart"/> instances that a quote have been generated for.
        /// </summary>
        /// <remarks>
        /// <note type="Warning">
        /// The predicate filter is added to the provided <paramref name="queryable"/> instance. 
        /// It does not guarantee that a <see cref="Cart"/> instance will be returned if the originating
        /// predicate produces no results.
        /// 
        /// In addition, this predicate does not limit filters to specific cart instances and therefore is best utilized
        /// with source queries that perform this logic. Allows callers to filter carts by the presence of an existing
        /// <see cref="CsvCart.Quote"/> value on an otherwise unqueryable property.
        /// </note>
        /// </remarks>
        /// <param name="queryable">The source query to acquire the data from.</param>
        /// <returns>A new <see cref="IQueryable{Cart}"/> that can be further customized or used.</returns>
        public static IQueryable<Cart> WithGeneratedQuote(this IQueryable<Cart> queryable)
        {
            if (queryable == null) throw new ArgumentNullException(nameof(queryable));
            Contract.Ensures(Contract.Result<IQueryable<Cart>>() != null);
            Contract.EndContractBlock();

            queryable = queryable.Where(c => c.QuoteInternal != null);
            return queryable;
        }
        #endregion

        #region Types

        /// <summary>
        /// Crafts a query predicate that can acquire the <see cref="Cart"/> instance for NationBuilder orders.
        /// </summary>
        /// <remarks>
        /// <note type="Warning">
        /// The predicate filter is added to the provided <paramref name="queryable"/> instance. 
        /// It does not guarantee that a <see cref="Cart"/> instance will be returned if the originating
        /// predicate produces no results. 
        /// </note>
        /// </remarks>
        /// <param name="queryable">The source query to acquire the data from.</param>
        /// <param name="cartId">The optional identifier of the cart that should be returned.</param>
        /// <returns>A queryable that can return carts for NationBuilder orders.</returns>
        public static IQueryable<NationBuilderCart> ForNationBuilder(this IQueryable<Cart> queryable, Guid? cartId = null)
        {
            if (queryable == null) throw new ArgumentNullException(nameof(queryable));
            Contract.Ensures(Contract.Result<IQueryable<NationBuilderCart>>() != null);
            Contract.EndContractBlock();

            if (cartId != null) queryable = queryable.Where(c => c.Id == cartId.Value);
            return queryable.OfType<NationBuilderCart>();
        }

        /// <summary>
        /// Crafts a query predicate that can acquire the <see cref="Cart"/> instance for CSV upload orders.
        /// </summary>
        /// <remarks>
        /// <note type="Warning">
        /// The predicate filter is added to the provided <paramref name="queryable"/> instance. 
        /// It does not guarantee that a <see cref="Cart"/> instance will be returned if the originating
        /// predicate produces no results. 
        /// </note>
        /// </remarks>
        /// <param name="queryable">The source query to acquire the data from.</param>
        /// <param name="cartId">The optional identifier of the cart that should be returned.</param>
        /// <returns>A queryable that can return carts for CSV upload orders.</returns>
        public static IQueryable<CsvCart> ForCsv(this IQueryable<Cart> queryable, Guid? cartId = null)
        {
            if (queryable == null) throw new ArgumentNullException(nameof(queryable));
            Contract.Ensures(Contract.Result<IQueryable<CsvCart>>() != null);
            Contract.EndContractBlock();

            if (cartId != null) queryable = queryable.Where(c => c.Id == cartId.Value);
            return queryable.OfType<CsvCart>();
        }
        
        /// <summary>
        /// Crafts a query predicate that can acquire the <see cref="Cart"/> instance for List Builder based orders.
        /// </summary>
        /// <remarks>
        /// <note type="Warning">
        /// The predicate filter is added to the provided <paramref name="queryable"/> instance. 
        /// It does not guarantee that a <see cref="Cart"/> instance will be returned if the originating
        /// predicate produces no results. 
        /// </note>
        /// </remarks>
        /// <param name="queryable">The source query to acquire the data from.</param>
        /// <param name="cartId">The optional identifier of the cart that should be returned.</param>
        /// <returns>A queryable that can return carts for List Builder based orders.</returns>
        public static IQueryable<Cart> ForListBuilder(this IQueryable<Cart> queryable, Guid? cartId = null)
        {
            if (queryable == null) throw new ArgumentNullException(nameof(queryable));
            Contract.Ensures(Contract.Result<IQueryable<Cart>>() != null);
            Contract.EndContractBlock();

            throw new NotSupportedException();
            //queryable = queryable.Where(c => c.Source == FileSource.ListBuilder);

            //return queryable.OfType<ListBuilderCart>();
        }

        #endregion
    }
}