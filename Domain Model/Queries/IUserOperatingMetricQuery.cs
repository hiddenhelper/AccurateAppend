using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core;

namespace DomainModel.Queries
{
    /// <summary>
    /// Report containing top level <see cref="AccurateAppend.Accounting.Deal"/> metrics.
    /// </summary>
    public interface IUserOperatingMetricQuery
    {
        /// <summary>
        /// Initializes a new queryable against activity of a User.
        /// </summary>
        /// <param name="applicationId">The identifier of the application to query metrics for.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> used to signal that an operation should be canceled.</param>
        /// <returns>A new queryable that can be further customized.</returns>
        Task<IEnumerable<UserOperatingMetricRecord>> Query(Guid applicationId, CancellationToken cancellation);
    }

    /// <summary>
    /// Operating metrics
    /// </summary>
    [Serializable()]
    public enum UserOperatingMetricName
    {
        /// <summary>
        /// Total number of Jobs
        /// </summary>
        [Description("Jobs")]
        TotalJobs = 1,
        /// <summary>
        /// Total Number of Input Records
        /// </summary>
        [Description("Input Records")]
        TotalRecords = 2,
        /// <summary>
        /// Total Number of Record Matches
        /// </summary>
        [Description("Record Matches")]
        TotalRecordMatches = 3,
        /// <summary>
        /// Total number of deals
        /// </summary>
        [Description("Deals")]
        TotalNumberOfDeals = 4,
        /// <summary>
        /// Subscriber revenue
        /// </summary>
        [Description("Self-service Revenue")]
        SelfServiceRevenue = 5,
        /// <summary>
        /// Subscriber revenue
        /// </summary>
        [Description("Charge Events")]
        TotalChargeEvents = 6,
        /// <summary>
        /// Non Subscriber revenue
        /// </summary>
        [Description("Charge Events Revenue")]
        ChargeEventsRevenue = 7,
        /// <summary>
        /// Total revenue
        /// </summary>
        [Description("Total Revenue")]
        TotalRevenue = 8,
        /// <summary>
        /// Total Api Calls
        /// </summary>
        [Description("Total Api Calls")]
        TotalApiCalls = 9,
        /// <summary>
        /// Total Api Calls
        /// </summary>
        [Description("Total Api Matches")]
        TotalApiMatches = 10,
    }

    /// <summary>
    /// Top level measure of specific account metrics
    /// </summary>
    public class UserOperatingMetricRecord
    {
        /// <summary>
        /// Used in ORM or subclassing scenarios.
        /// </summary>
        protected UserOperatingMetricRecord()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetricRecord"/> class.
        /// </summary>
        public UserOperatingMetricRecord(UserOperatingMetricName metricName)
        {
            this.MetricName = metricName;
        }

        /// <summary>
        /// Description of metric
        /// </summary>
        public UserOperatingMetricName MetricName { get; protected set; }

        /// <summary>
        /// Returns plain text description of MetricName property
        /// </summary>
        public String MetricNameDescription => this.MetricName.GetDescription();

        /// <summary>
        /// All activity for the current day starting at the most recent midnight
        /// </summary>
        public Decimal Today { get; set; }

        /// <summary>
        /// All activity for the prior day ending at the most recent midnight
        /// </summary>
        public Decimal Yesterday { get; set; }

        /// <summary>
        /// All activity for the prior 7 days ending at the most recent midnight
        /// </summary>
        public Decimal Last7 { get; set; }

        /// <summary>
        /// All activity for the current month
        /// </summary>
        public Decimal CurrentMonth { get; set; }

        /// <summary>
        /// All activity for last month 
        /// </summary>
        public Decimal LastMonth { get; set; }

        /// <summary>
        /// All activity for same date range last month 
        /// </summary>
        public Decimal SamePeriodLastMonth { get; set; }

        /// <summary>
        /// All activity for the month prior to the last month
        /// </summary>
        public Decimal PreviousToLastMonth { get; set; }
    }
}