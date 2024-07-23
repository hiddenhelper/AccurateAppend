using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DomainModel.Queries
{
    /// <summary>
    /// Report containing top level Deal  metrics.
    /// </summary>
    public interface IUserDealMetricQuery
    {
        /// <summary>
        /// Intializes a new queryable against the deal metrics for the indicated application.
        /// </summary>
        /// <param name="applicationId">The identifier of the application to query metrics for.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> used to signal that an operation should be canceled.</param>
        /// <returns>A new queryable that can be further customized.</returns>
        Task<IEnumerable<UserDealMetricRecord>> Query(Guid applicationId, CancellationToken cancellation);
    }

    /// <summary>
    /// Top level measure of specific account metrics
    /// </summary>
    public class UserDealMetricRecord
    {
        /// <summary>
        /// Used in ORM or subclassing scenarios.
        /// </summary>
        protected UserDealMetricRecord()
        {
        }
        
        /// <summary>
        /// Description of metric
        /// </summary>
        public string Email { get; protected set; }

        /// <summary>
        /// Unique cuustomer identifier
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// All activity for the current day starting at the most recent midnight
        /// </summary>
        public Decimal TodayRevenue { get; set; }

        /// <summary>
        /// All activity for the prior day ending at the most recent midnight
        /// </summary>
        public Decimal YesterdayRevenue { get; set; }

        /// <summary>
        /// All activity for the prior 7 days ending at the most recent midnight
        /// </summary>
        public Decimal Last7Revenue { get; set; }
        
        /// <summary>
        /// All activity for the current month
        /// </summary>
        public Decimal CurrentMonthRevenue { get; set; }
        
        /// <summary>
        /// All activity for last month 
        /// </summary>
        public Decimal LastMonthRevenue { get; set; }
        
        /// <summary>
        /// All activity for same date range last month 
        /// </summary>
        public Decimal SamePeriodLastMonthRevenue { get; set; }
        
        /// <summary>
        /// All activity for the month prior to the last month
        /// </summary>
        public Decimal PreviousToLastMonthRevenue { get; set; }

        /// <summary>
        /// All activity for the prior rolling 12 months
        /// </summary>
        public Decimal Rolling12MonthRevenue { get; set; }
    }
}