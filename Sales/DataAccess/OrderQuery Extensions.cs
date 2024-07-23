#pragma warning disable SA1649 // File name must match first type name

using System;
using System.Linq;
using System.Threading;
using AccurateAppend.Core.IdentityModel;

namespace AccurateAppend.Sales.DataAccess
{
    /// <summary>
    /// Query extensions for the <see cref="Order"/> class.
    /// </summary>
    public static class OrderQueryExtensions
    {
        /// <summary>
        /// Appends predicate logic to filter <see cref="Order"/> instances that are editable.
        /// </summary>
        /// <remarks>
        /// This extension method does not ensure that the base query is logical with this filter predicate
        /// and no guarantees that the realization of the returned query will return any items.
        /// </remarks>
        /// <param name="baseQuery">The source data to filter.</param>
        /// <returns>A new <see cref="IQueryable{T}"/> that can be further customized or used.</returns>
        public static IQueryable<T> AreEditable<T>(this IQueryable<T> baseQuery) where T : Order
        {
            if (baseQuery == null) throw new ArgumentNullException(nameof(baseQuery));

            return baseQuery.Where(d => OrderExtensions.EditableStatus.Contains(d.Status));
        }

        /// <summary>
        /// Appends predicate logic to filter <see cref="Order"/> instances that are completed. That is an <see cref="Order"/>
        /// that cannot have additional modifications performed.
        /// </summary>
        /// <remarks>
        /// This extension method does not ensure that the base query is logical with this filter predicate
        /// and no guarantees that the realization of the returned query will return any items.
        /// </remarks>
        /// <param name="baseQuery">The source data to filter.</param>
        /// <returns>A new <see cref="IQueryable{T}"/> that can be further customized or used.</returns>
        public static IQueryable<T> AreComplete<T>(this IQueryable<T> baseQuery) where T : Order
        {
            if (baseQuery == null) throw new ArgumentNullException(nameof(baseQuery));

            return baseQuery.Where(d => OrderExtensions.CompletedStatus.Contains(d.Status));
        }

        /// <summary>
        /// Appends predicate logic to filter <see cref="Order"/> instances that are for the current interactive identity.
        /// </summary>
        /// <remarks>
        /// This extension method does not ensure that the base query is logical with this filter predicate
        /// and no guarantees that the realization of the returned query will return any items.
        /// </remarks>
        /// <param name="baseQuery">The source data to filter.</param>
        /// <returns>A new <see cref="IQueryable{T}"/> that can be further customized or used.</returns>
        public static IQueryable<T> ForInteractiveUser<T>(this IQueryable<T> baseQuery) where T : Order
        {
            if (baseQuery == null) throw new ArgumentNullException(nameof(baseQuery));

            var userId = Thread.CurrentPrincipal.Identity.GetIdentifier();

            return baseQuery.Where(o => o.Deal.Client.UserId == userId);
        }
    }
}
