using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DomainModel.Queries
{
    /// <summary>
    /// Report containing top level DataServiceOperation metrics.
    /// </summary>
    public interface IUserProcessingMetricQuery
    {
        /// <summary>
        /// Intializes a new queryable against the deal metrics for the indicated application.
        /// </summary>
        /// <param name="applicationId">The identifier of the application to query metrics for.</param>
        /// <param name="source">The identifier of the system to which the metrics belong. Batch, Xml</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> used to signal that an operation should be canceled.</param>
        /// <returns>A new queryable that can be further customized.</returns>
        Task<IEnumerable<UserProcessingMetricRecord>> Query(Guid applicationId, Source source, CancellationToken cancellation);
    }

    /// <summary>
    /// Top level measure of specific account metrics
    /// </summary>
    public class UserProcessingMetricRecord
    {
        /// <summary>
        /// Used in ORM or subclassing scenarios.
        /// </summary>
        protected UserProcessingMetricRecord()
        {
        }
        
        /// <summary>
        /// Email address of User
        /// </summary>
        public string Email { get; protected set; }

        /// <summary>
        /// UserId of User
        /// </summary>
        public Guid UserId { get; protected set; }

        /// <summary>
        /// All activity for the current day starting at the most recent midnight
        /// </summary>
        public int TodayRecords { get; set; }

        /// <summary>
        /// All activity for the current day starting at the most recent midnight
        /// </summary>
        public int TodayMatches { get; set; }

        /// <summary>
        /// All activity for the prior day ending at the most recent midnight
        /// </summary>
        public int YesterdayRecords { get; set; }

        /// <summary>
        /// All activity for the prior day ending at the most recent midnight
        /// </summary>
        public int YesterdayMatches { get; set; }

        /// <summary>
        /// All activity for the prior 7 days ending at the most recent midnight
        /// </summary>
        public int Last7Records { get; set; }

        /// <summary>
        /// All activity for the prior 7 days ending at the most recent midnight
        /// </summary>
        public int Last7Matches { get; set; }

        /// <summary>
        /// All activity for the current month
        /// </summary>
        public int CurrentMonthRecords { get; set; }

        /// <summary>
        /// All activity for the current month
        /// </summary>
        public int CurrentMonthMatches { get; set; }

        /// <summary>
        /// All activity for last month 
        /// </summary>
        public int LastMonthRecords { get; set; }

        /// <summary>
        /// All activity for last month 
        /// </summary>
        public int LastMonthMatches { get; set; }

        /// <summary>
        /// All activity for same date range last month 
        /// </summary>
        public int SamePeriodLastMonthRecords { get; set; }

        /// <summary>
        /// All activity for same date range last month 
        /// </summary>
        public int SamePeriodLastMonthMatches { get; set; }

        /// <summary>
        /// All activity for the month prior to the last month
        /// </summary>
        public int PreviousToLastMonthRecords { get; set; }

        /// <summary>
        /// All activity for the month prior to the last month
        /// </summary>
        public int PreviousToLastMonthMatches { get; set; }

        /// <summary>
        /// All activity for the rolling 12 months
        /// </summary>
        public int Rolling12MonthRecords { get; set; }

        /// <summary>
        /// All activity for the rolling 12 months
        /// </summary>
        public int Rolling12MonthMatches { get; set; }
    }
}