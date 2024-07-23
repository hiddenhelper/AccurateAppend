#pragma warning disable SA1649 // File name must match first type name

using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace AccurateAppend.Sales.DataAccess
{
    /// <summary>
    /// Query extensions for the <see cref="TransactionEvent"/> class.
    /// </summary>
    public static class ChargeEventQueryExtensions
    {
        /// <summary>
        /// Filters the <see cref="TransactionEvent"/> sequence to only those items that posted a captured amount.
        /// </summary>
        /// <param name="events">The sequence to filter.</param>
        /// <returns>The filtered sequence of <see cref="TransactionEvent"/> instances.</returns>
        public static IQueryable<TransactionEvent> AreCaptured(this IQueryable<TransactionEvent> events)
        {
            if (events == null) throw new ArgumentNullException(nameof(events));
            Contract.EndContractBlock();

            return events.Where(e => TransactionResultExtensions.CapturedStatus.Contains(e.Status));
        }

        /// <summary>
        /// Filters the <see cref="TransactionEvent"/> sequence to only those items that didn't post a captured amount.
        /// </summary>
        /// <param name="events">The sequence to filter.</param>
        /// <returns>The filtered sequence of <see cref="TransactionEvent"/> instances.</returns>
        public static IQueryable<TransactionEvent> AreNotCaptured(this IQueryable<TransactionEvent> events)
        {
            if (events == null) throw new ArgumentNullException(nameof(events));
            Contract.EndContractBlock();

            return events.Where(e => !TransactionResultExtensions.CapturedStatus.Contains(e.Status));
        }
    }
}
