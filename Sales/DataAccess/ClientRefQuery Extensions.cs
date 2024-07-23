#pragma warning disable SA1649 // File name must match first type name
using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;

namespace AccurateAppend.Sales.DataAccess
{
    /// <summary>
    /// Provides extension methods for the <see cref="ClientRef"/> entity.
    /// </summary>
    public static class ClientRefQueryExtensions
    {
        /// <summary>
        /// Crafts a query predicate that can acquire the <see cref="ClientRef"/> account for the current interactive user.
        /// </summary>
        /// <remarks>
        /// <note type="Warning">
        /// The predicate filter is added to the provided <paramref name="queryable"/> instance. 
        /// It does not guarantee that a <see cref="ClientRef"/> instance will be returned if the originating
        /// predicate produces no results. 
        /// </note>
        /// </remarks>
        /// <param name="queryable">The source query to acquire the data from.</param>
        /// <returns>A queryable that can return the Client account for the current interactive user.</returns>
        public static IQueryable<ClientRef> ForInteractiveUser(this IQueryable<ClientRef> queryable)
        {
            if (queryable == null) throw new ArgumentNullException(nameof(queryable));
            Contract.Ensures(Contract.Result<IQueryable<ClientRef>>() != null);
            Contract.EndContractBlock();

            var username = Thread.CurrentPrincipal.Identity.Name;
            return queryable.Where(u => u.UserName == username);
        }
    }
}
