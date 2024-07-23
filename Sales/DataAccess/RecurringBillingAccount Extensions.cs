#pragma warning disable SA1649 // File name must match first type name

using System;
using System.Linq;
using AccurateAppend.Core;

namespace AccurateAppend.Sales.DataAccess
{
    /// <summary>
    /// Query extensions for the <see cref="RecurringBillingAccount"/> class.
    /// </summary>
    public static class RecurringBillingAccountExtensions
    {
        /// <summary>
        /// Appends predicate logic to filter <see cref="RecurringBillingAccount"/> instance that
        /// are valid for a given date for an indicated user identifier.
        /// </summary>
        /// <remarks>
        /// This extension method does not ensure that the base query is logical with this filter predicate
        /// and no guarantees that the realization of the returned query will return any items.
        /// </remarks>
        /// <param name="baseQuery">The source data to filter.</param>
        /// <param name="userId">The identifier of the user to return <see cref="RecurringBillingAccount"/> instances for.</param>
        /// <param name="date">The <see cref="DateTime"/> the desired <seealso cref="RecurringBillingAccount"/> should be valid for. This value is converted to billing zone and only the <see cref="DateTime.Date"/> value is used.</param>
        /// <returns>A new <see cref="IQueryable{RecurringBillingAccount}"/> that can be further customized or used.</returns>
        public static IQueryable<RecurringBillingAccount> ValidForDate(this IQueryable<RecurringBillingAccount> baseQuery, Guid userId, DateTime date)
        {
            if (baseQuery == null) throw new ArgumentNullException(nameof(baseQuery));

            var effectiveDate = date.ToBillingZone().Date;

            var query = baseQuery
                .Where(a => a.ForClient.UserId == userId)
                .Where(a => a.EffectiveDate <= effectiveDate && (a.EndDate == null || a.EndDate >= effectiveDate));

            return query;
        }
    }
}
