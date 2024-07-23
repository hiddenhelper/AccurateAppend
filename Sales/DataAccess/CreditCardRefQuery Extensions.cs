#pragma warning disable SA1649 // File name must match first type name
using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;

namespace AccurateAppend.Sales.DataAccess
{
    /// <summary>
    /// Provides extension methods for the <see cref="CreditCardRef"/> entity.
    /// </summary>
    public static class CreditCardRefQueryExtensions
    {
        /// <summary>
        /// Crafts a query predicate that can acquire the <see cref="CreditCardRef"/> instances for the current interactive user.
        /// </summary>
        /// <remarks>
        /// <note type="Warning">
        /// The predicate filter is added to the provided <paramref name="queryable"/> instance. 
        /// It does not guarantee that a <see cref="CreditCardRef"/> instance will be returned if the originating
        /// predicate produces no results. 
        /// </note>
        /// </remarks>
        /// <param name="queryable">The source query to acquire the data from.</param>
        /// <returns>A queryable that can return the <see cref="CreditCardRef"/> for the current interactive user.</returns>
        public static IQueryable<CreditCardRef> CardsForInteractiveUserAsync(this IQueryable<CreditCardRef> queryable)
        {
            if (queryable == null) throw new ArgumentNullException(nameof(queryable));
            Contract.Ensures(Contract.Result<IQueryable<CreditCardRef>>() != null);
            Contract.EndContractBlock();

            var username = Thread.CurrentPrincipal.Identity.Name;
            var query = queryable.Where(c => c.Client.UserName == username);

            return query;
        }
    }
}
