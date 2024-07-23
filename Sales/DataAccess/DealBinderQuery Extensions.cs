#pragma warning disable SA1649 // File name must match first type name

using System;
using System.Linq;

namespace AccurateAppend.Sales.DataAccess
{
    /// <summary>
    /// Query extensions for the <see cref="DealBinder"/> class.
    /// </summary>
    public static class DealBinderQueryExtensions
    {
        /// <summary>
        /// Appends predicate logic to filter <see cref="DealBinder"/> that are editable.
        /// </summary>
        /// <remarks>
        /// This extension method does not ensure that the base query is logical with this filter predicate
        /// and no guarantees that the realization of the returned query will return any items.
        /// </remarks>
        /// <param name="baseQuery">The source data to filter.</param>
        /// <returns>A new <see cref="IQueryable{DealBinder}"/> that can be further customized or used.</returns>
        public static IQueryable<DealBinder> AreEditable(this IQueryable<DealBinder> baseQuery)
        {
            if (baseQuery == null) throw new ArgumentNullException(nameof(baseQuery));

            return baseQuery.Where(d => DealStatusExtensions.EditableStatus.Contains(d.Status));
        }

        /// <summary>
        /// Appends predicate logic to filter <see cref="DealBinder"/> that are in the approval state.
        /// </summary>
        /// <remarks>
        /// This extension method does not ensure that the base query is logical with this filter predicate
        /// and no guarantees that the realization of the returned query will return any items.
        /// </remarks>
        /// <param name="baseQuery">The source data to filter.</param>
        /// <returns>A new <see cref="IQueryable{DealBinder}"/> that can be further customized or used.</returns>
        public static IQueryable<DealBinder> AreInApproval(this IQueryable<DealBinder> baseQuery)
        {
            if (baseQuery == null) throw new ArgumentNullException(nameof(baseQuery));

            return baseQuery.Where(d => DealStatusExtensions.AprrovalStatus.Contains(d.Status));
        }

        /// <summary>
        /// Appends predicate logic to filter <see cref="DealBinder"/> that are able to be refunded.
        /// </summary>
        /// <remarks>
        /// This extension method does not ensure that the base query is logical with this filter predicate
        /// and no guarantees that the realization of the returned query will return any items.
        /// </remarks>
        /// <param name="baseQuery">The source data to filter.</param>
        /// <returns>A new <see cref="IQueryable{DealBinder}"/> that can be further customized or used.</returns>
        public static IQueryable<T> CanBeRefunded<T>(this IQueryable<T> baseQuery) where T : DealBinder
        {
            if (baseQuery == null) throw new ArgumentNullException(nameof(baseQuery));

            return baseQuery.Where(d => d.Status == DealStatus.Complete && d.Amount > 0M);
        }

        /// <summary>
        /// Appends predicate logic to filter <see cref="DealBinder"/> that are able to be billed.
        /// </summary>
        /// <remarks>
        /// This extension method does not ensure that the base query is logical with this filter predicate
        /// and no guarantees that the realization of the returned query will return any items.
        /// </remarks>
        /// <param name="baseQuery">The source data to filter.</param>
        /// <returns>A new <see cref="IQueryable{DealBinder}"/> that can be further customized or used.</returns>
        public static IQueryable<T> CanBeBilled<T>(this IQueryable<T> baseQuery) where T : DealBinder
        {
            if (baseQuery == null) throw new ArgumentNullException(nameof(baseQuery));

            return baseQuery.Where(d => d.Status == DealStatus.Billing);
        }
    }
}
