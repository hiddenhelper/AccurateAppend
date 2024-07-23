using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core;
using Newtonsoft.Json;
using DateTimeConverter = DomainModel.JsonNET.DateTimeConverter;

namespace DomainModel.Queries
{
    /// <summary>
    /// Report containing top level <see cref="AccurateAppend.Sales.DealBinder"/> metrics.
    /// </summary>
    public interface IOperatingMetricQuery
    {
        /// <summary>
        /// Initializes a new queryable against the deal metrics for the indicated application.
        /// </summary>
        /// <param name="applicationId">The identifier of the application to query metrics for.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> used to signal that an operation should be canceled.</param>
        /// <returns>A new queryable that can be further customized.</returns>
        Task<IEnumerable<MetricRecord>> Query(Guid applicationId, CancellationToken cancellation = default(CancellationToken));

        Task<IEnumerable<RevenueMetric>> GenerateRevenueMetrics(Guid applicationId, DateTime startDate, DateTime endDate, CancellationToken cancellation = default(CancellationToken));
    }

    /// <summary>
    /// Operating metrics
    /// </summary>
    [Serializable()]
    public enum MetricName
    {
        /// <summary>
        /// Total number of qualified leads
        /// </summary>
        [Description("Unique Website Users (www)")]
        UnqiueWebsiteUsersWww = -2,

        /// <summary>
        /// Total number of qualified leads
        /// </summary>
        [Description("Unique Website Users (clients)")]
        UnqiueWebsiteUsersClients = -1,

        /// <summary>
        /// Total number of qualified leads
        /// </summary>
        [Description("Qualified Leads")]
        TotalNumberOfQualfiedLeads = 1,

        /// <summary>
        /// Total number of new users
        /// </summary>
        [Description("New Users")]
        TotalNumberOfNewUsers = 2,

        /// <summary>
        /// Count of distinct users
        /// </summary>
        [Description("Active Users")]
        DistinctUsers = 3,

        /// <summary>
        /// Count of distinct subscribers
        /// </summary>
        [Description("Active Subscribers")]
        DistinctSubscribers = 4,

        /// <summary>
        /// Total number of deals
        /// </summary>
        [Description("Total Number of Jobs")]
        TotalJobs = 5,

        /// <summary>
        /// Total Number of Jobs (Unique User)
        /// </summary>
        [Description("Total Number of Jobs (Unique User)")]
        TotalJobsDistinctUser = 6,

        /// <summary>
        /// Total Number of Input Records
        /// </summary>
        [Description("Total Number of Input Records")]
        TotalRecords = 7,

        /// <summary>
        /// Total Number of Record Matches
        /// </summary>
        [Description("Total Number of Record Matches")]
        TotalRecordMatches = 8,

        /// <summary>
        /// Total number of deals
        /// </summary>
        [Description("Total Number of Deals")]
        TotalNumberOfDeals = 9,

        /// <summary>
        /// Average deal amount
        /// </summary>
        [Description("Average Deal Amount")]
        AverageDealAmount = 10,

        /// <summary>
        /// Subscriber revenue
        /// </summary>
        [Description("Subscriber Revenue")]
        SubscriberRevenue = 11,

        /// <summary>
        /// Non Subscriber revenue
        /// </summary>
        [Description("Non Subscriber Revenue")]
        NonSubscriberRevenue = 12,

        /// <summary>
        /// Subscriber revenue
        /// </summary>
        [Description("NationBuilder Revenue")]
        NationBuilderRevenue = 13,

        /// <summary>
        /// Subscriber revenue
        /// </summary>
        [Description("Self-service Revenue")]
        SelfServiceRevenue = 14,

        /// <summary>
        /// Subscriber revenue
        /// </summary>
        [Description("Charge Events")]
        TotalChargeEvents = 15,

        /// <summary>
        /// Non Subscriber revenue
        /// </summary>
        [Description("Charge Events Revenue")]
        ChargeEventsRevenue = 16,

        /// <summary>
        /// Total revenue
        /// </summary>
        [Description("Total Revenue")]
        TotalRevenue = 17
    }

    /// <summary>
    /// Top level measure of specific account metrics
    /// </summary>
    public class MetricRecord
    {
        /// <summary>
        /// Used in ORM or subclassing scenarios.
        /// </summary>
        protected MetricRecord()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetricRecord"/> class.
        /// </summary>
        public MetricRecord(MetricName metricName)
        {
            this.MetricName = metricName;
        }

        /// <summary>
        /// Description of metric
        /// </summary>
        public MetricName MetricName { get; protected set; }

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

    public class RevenueMetric
    {
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime Date { get; set; }

        [DefaultValue(0)]
        public decimal? TotalRevenue { get; set; }

        [DefaultValue(0)]
        public decimal? RevenueSubscriber { get; set; }

        [DefaultValue(0)]
        public int DealsSubscriber { get; set; }

        [DefaultValue(0)]
        public decimal? AvgDealAmountSubscriber { get; set; }

        [DefaultValue(0)]
        public decimal? RevenueNonSubscriber { get; set; }

        [DefaultValue(0)]
        public int DealsNonSubscriber { get; set; }

        [DefaultValue(0)]
        public decimal? AvgDealAmountNonSubscriber { get; set; }

        public decimal? RevenueNationBuilder { get; set; }

        public int DealsNationBuilder { get; set; }

        public decimal? AvgDealAmountNationBuilder { get; set; }

        /// <inheritdoc />
        public override String ToString()
        {
            return $"[RevenueMetric: Date={this.Date}, TotalRevenue={this.TotalRevenue}, RevenueSubscriber={this.RevenueSubscriber}, DealsSubscriber={this.DealsSubscriber}, AvgDealAmountSubscriber={this.AvgDealAmountSubscriber}, RevenueNonSubscriber={this.RevenueNonSubscriber}, DealsNonSubscriber={this.DealsNonSubscriber}, AvgDealAmountNonSubscriber={this.AvgDealAmountNonSubscriber}, RevenueNonSubscriber={this.RevenueNationBuilder}, DealsNonSubscriber={this.DealsNationBuilder}, AvgDealAmountNonSubscriber={this.AvgDealAmountNationBuilder}]";
        }
    }
}