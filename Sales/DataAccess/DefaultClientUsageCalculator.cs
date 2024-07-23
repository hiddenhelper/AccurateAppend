using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core;

namespace AccurateAppend.Sales.DataAccess
{
    /// <summary>
    /// Default implementation of <see cref="IClientUsageCalculator"/> component.
    /// Uses a supplied <see cref="DefaultContext"/> that can provide data access
    /// to the <see cref="LedgerEntry"/> tally.
    /// </summary>
    public class DefaultClientUsageCalculator : IClientUsageCalculator
    {
        #region Fields

        private readonly DefaultContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultClientUsageCalculator"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> that should be used with the component.</param>
        public DefaultClientUsageCalculator(DefaultContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region IClientUsageCalculator Members

        /// <inheritdoc />
        public virtual Task<IEnumerable<UseReport>> Calculate(Guid userId, DateSpan period, CancellationToken cancellation)
        {
            period = period.ToBillingZone();
            period.TrimTime();

            const String Sql = @"
            SELECT[Key][Source], Sum([Count]) [RecordCount], Sum([Matches]) [MatchCount]
            FROM[sales].[UsageRollup]
            WHERE[UserId] = @p0 AND
                [RollupDate] >= @p1 AND[RollupDate] <= @p2 AND
                [Source] IN(0,1) -- Batch Usage, API Usage
                GROUP BY[Key]";

            return this.context
                .Database
                .SqlQuery<UseReport>(Sql, userId, period.StartingOn, period.EndingOn)
                .ToArrayAsync(cancellation)
                .ContinueWith(t => (IEnumerable<UseReport>) t.Result, cancellation, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Current);
        }

        /// <inheritdoc />
        public virtual Task<DateTime?> NewestLedgerDate(Int32 accountId, LedgerType classification, CancellationToken cancellation = default(CancellationToken))
        {
            return this.context.Database.SqlQuery<DateTime?>(
                    @"
SELECT Max(EffectiveDate) FROM [sales].[AccountLedger] al
INNER JOIN [sales].[Subscriptions] s ON al.[AccountId]=s.[SubscriptionId]
WHERE al.[AccountId]=@p0 AND (al.[EntryType]=@p1 OR al.[EntryType]=@p2)",
                    accountId,
                    (Int32) classification,
                    (Int32) LedgerType.GeneralAdjustment)
                .FirstOrDefaultAsync(cancellation);
        }

        ///// <inheritdoc />        
        //public virtual Task<IEnumerable<LedgerEntry>> LedgersForPeriod(Guid userId, DateSpan period, CancellationToken cancellation = default(CancellationToken))
        //{
        //    period = period.ToBillingZone();
        //    period.TrimTime();

        //    var query = this.context.SetOf<LedgerEntry>()
        //        .Where(e => e.ForAccount.ForClient.UserId == userId)
        //        .Where(e => e.EffectiveDate >= period.StartingOn && e.EffectiveDate <= period.EndingOn);
        //    query = query.Include(e => e.WithDeal).Include(e => e.WithDeal.Orders);

        //    var task = query
        //        .ToArrayAsync(cancellation)
        //        .ContinueWith(t => (IEnumerable<LedgerEntry>) t.Result, cancellation);
        //    return task;
        //}

        #endregion
    }
}