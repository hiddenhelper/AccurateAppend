#pragma warning disable SA1402 // File may only contain a single class
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Represents a component capable of tallying usage over a period for a client
    /// and for providing direct ledger access.
    /// </summary>
    /// <remarks>
    /// Implementors can confirm that all <see cref="DateSpan"/> input is converted to
    /// <see cref="DateTimeExtensions.ToBillingZone(DateTime)">billing time</see> and
    /// only operates at <see cref="DateSpanExtensions.TrimTime">date grain</see>.
    /// </remarks>
    [ContractClass(typeof(IClientUsageCalculatorContracts))]
    public interface IClientUsageCalculator
    {
        /// <summary>
        /// Tallies and compiles the total use for a client over a period by operation. All calculations are performed in 
        /// <see cref="DateTimeExtensions.ToBillingZone(DateTime)">billing time</see> by date granularity.
        /// </summary>
        /// <param name="userId">The identifier of the client to aggregate.</param>
        /// <param name="period">The business dates (inclusive) to query through.</param>
        /// <param name="cancellation">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>The aggregated usage by product.</returns>
        Task<IEnumerable<UseReport>> Calculate(Guid userId, DateSpan period, CancellationToken cancellation = default(CancellationToken));

        /// <summary>
        /// Indicates the newest ledger date of the indicated type on a client ledger. All calculations are performed in 
        /// <see cref="DateTimeExtensions.ToBillingZone(DateTime)">billing time</see> by date granularity.
        /// </summary>
        /// <param name="accountId">The identifier of the client <see cref="RecurringBillingAccount"/> to find ledger dates for.</param>
        /// <param name="classification">The type of entry to the ledger.</param>
        /// <param name="cancellation">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>The newest date (inclusive), if any; Otherwise null.</returns>
        Task<DateTime?> NewestLedgerDate(Int32 accountId, LedgerType classification, CancellationToken cancellation = default(CancellationToken));

        ///// <summary>
        ///// Queries for all <see cref="LedgerEntry"/> for a client within a certain period. All calculations are performed in 
        ///// <see cref="DateTimeExtensions.ToBillingZone(DateTime)">billing time</see> by date granularity.
        ///// </summary>
        ///// <param name="userId">The identifier of the client to aggregate.</param>
        ///// <param name="period">The business dates (inclusive) to query through.</param>
        ///// <param name="cancellation">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
        ///// <returns>The sequence of all <see cref="LedgerEntry"/> in the period for the client, irrespective of type and <see cref="ServiceAccount"/>.</returns>
        //Task<IEnumerable<LedgerEntry>> LedgersForPeriod(Guid userId, DateSpan period, CancellationToken cancellation = default(CancellationToken));
    }

    // ReSharper disable InconsistentNaming
    [ContractClassFor(typeof(IClientUsageCalculator))]
    internal abstract class IClientUsageCalculatorContracts : IClientUsageCalculator
    {
        Task<IEnumerable<UseReport>> IClientUsageCalculator.Calculate(Guid userId, DateSpan period, CancellationToken cancellation)
        {
            Contract.Requires<ArgumentNullException>(period != null, nameof(period));
            Contract.Requires<ArgumentException>(period.StartingOn <= period.EndingOn, nameof(period));
            Contract.EndContractBlock();

            return default(Task<IEnumerable<UseReport>>);
        }

        Task<DateTime?> IClientUsageCalculator.NewestLedgerDate(Int32 accountId, LedgerType classification, CancellationToken cancellation)
        {
            return default(Task<DateTime?>);
        }

        //Task<IEnumerable<LedgerEntry>> IClientUsageCalculator.LedgersForPeriod(Guid userId, DateSpan period, CancellationToken cancellation)
        //{
        //    Contract.Requires<ArgumentNullException>(period != null, nameof(period));
        //    Contract.Requires<ArgumentException>(period.StartingOn <= period.EndingOn, nameof(period));
        //    Contract.EndContractBlock();

        //    return default(Task<IEnumerable<LedgerEntry>>);
        //}
    }
    // ReSharper restore InconsistentNaming
}
