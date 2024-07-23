using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Accounting;
using AccurateAppend.Core;

namespace DomainModel.Queries
{
    /// <summary>
    /// Report containing top level <see cref="Lead"/> Lead metrics.
    /// </summary>
    public interface ILeadMetricQuery
    {
        /// <summary>
        /// Intializes a new queryable against the lead metrics for the indicated application.
        /// </summary>
        /// <param name="applicationId">The identifier of the application to query metrics for.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> used to signal that an operation should be canceled.</param>
        /// <returns>A new queryable that can be further customized.</returns>
        Task<IEnumerable<LeadMetricRecord>> Query(Guid applicationId, CancellationToken cancellation);
    }

    /// <summary>
    /// Top level measure of specific lead metrics
    /// </summary>
    public class LeadMetricRecord
    {
        /// <summary>
        /// Used in ORM or subclassing scenarios.
        /// </summary>
        protected LeadMetricRecord()
        {
        }

        public LeadMetricRecord(LeadSource leadSource)
        {
            this.LeadSource = leadSource;
        }

        /// <summary>
        /// Description of metric
        /// </summary>
        public LeadSource LeadSource { get; protected set; }

        /// <summary>
        /// Returns plain text description of MetricName property
        /// </summary>
        public String LeadSourceDescription => this.LeadSource.GetDescription();

        /// <summary>
        /// All activity for the current day starting at the most recent midnight
        /// </summary>
        public Decimal TodayLeadCount { get; set; }

        /// <summary>
        /// All activity for the current day starting at the most recent midnight
        /// </summary>
        public Decimal TodayNewClientCount { get; set; }

        /// <summary>
        /// All activity for the current day starting at the most recent midnight
        /// </summary>
        public Decimal TodayRevenueAmount { get; set; }

        /// <summary>
        /// All activity for the prior day ending at the most recent midnight
        /// </summary>
        public Decimal YesterdayLeadCount { get; set; }

        /// <summary>
        /// All activity for the prior day ending at the most recent midnight
        /// </summary>
        public Decimal YesterdayNewClientCount { get; set; }

        /// <summary>
        /// All activity for the prior day ending at the most recent midnight
        /// </summary>
        public Decimal YesterdayRevenueAmount { get; set; }

        /// <summary>
        /// All activity for the prior 7 days ending at the most recent midnight
        /// </summary>
        public Decimal Last7RecordsLeadCount { get; set; }

        /// <summary>
        /// All activity for the prior 7 days ending at the most recent midnight
        /// </summary>
        public Decimal Last7RecordsNewClientCount { get; set; }

        /// <summary>
        /// All activity for the prior 7 days ending at the most recent midnight
        /// </summary>
        public Decimal Last7RecordsRevenueAmount { get; set; }

        /// <summary>
        /// All activity for the current month
        /// </summary>
        public Decimal CurrentMonthLeadCount { get; set; }

        /// <summary>
        /// All activity for the current month
        /// </summary>
        public Decimal CurrentMonthNewClientCount { get; set; }

        /// <summary>
        /// All activity for the current month
        /// </summary>
        public Decimal CurrentMonthRevenueAmount { get; set; }

        /// <summary>
        /// All activity for same date range last month 
        /// </summary>
        public Decimal SamePeriodLastMonthLeadCount { get; set; }

        /// <summary>
        /// All activity for same date range last month 
        /// </summary>
        public Decimal SamePeriodLastMonthNewClientCount { get; set; }

        /// <summary>
        /// All activity for same date range last month 
        /// </summary>
        public Decimal SamePeriodLastMonthRevenueAmount { get; set; }

        /// <summary>
        /// All activity for last month 
        /// </summary>
        public Decimal LastMonthLeadCount { get; set; }

        /// <summary>
        /// All activity for last month 
        /// </summary>
        public Decimal LastMonthNewClientCount { get; set; }

        /// <summary>
        /// All activity for last month 
        /// </summary>
        public Decimal LastMonthRevenueAmount { get; set; }

        /// <summary>
        /// All activity for the month prior to the last month
        /// </summary>
        public Decimal PreviousToLastLeadCount { get; set; }

        /// <summary>
        /// All activity for the month prior to the last month
        /// </summary>
        public Decimal PreviousToLastNewClientCount { get; set; }

        /// <summary>
        /// All activity for the month prior to the last month
        /// </summary>
        public Decimal PreviousToLastRevenueAmount { get; set; }

        /// <summary>
        /// All activity for the last year
        /// </summary>
        public Decimal Rolling12MonthsLeadCount { get; set; }

        /// <summary>
        /// All activity for the last year
        /// </summary>
        public Decimal Rolling12MonthsNewClientCount { get; set; }

        /// <summary>
        /// All activity for the last year
        /// </summary>
        public Decimal Rolling12MonthsRevenueAmount { get; set; }

        /// <summary>
        /// Time to first purchase in days
        /// </summary>
        public Decimal TimeToFirstPurchase { get; set; }
    }
}