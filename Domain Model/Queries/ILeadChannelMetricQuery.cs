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
    /// Report containing top level <see cref="AccurateAppend.Accounting.Deal"/> metrics.
    /// </summary>
    public interface ILeadChannelMetricQuery
    {
        /// <summary>
        /// Intializes a new queryable against the monthly recurring revenue metrics for the indicated application.
        /// </summary>
        /// <param name="applicationId">The identifier of the application to query metrics for.</param>
        /// <param name="leadSource">The identifier the lead source.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> used to signal that an operation should be canceled.</param>
        /// <returns>A new queryable that can be further customized.</returns>
        Task<IEnumerable<LeadChannelMetricRecord>> Query(Guid applicationId, int leadSource, CancellationToken cancellation = default(CancellationToken));
    }

    /// <summary>
    /// Mrr Metrics
    /// </summary>
    [Serializable()]
    public enum LeadChannelMetricName
    {
        /// <summary>
        /// Leads for a specific channel
        /// </summary>
        [Description("Leads")]
        LeadCount = 0,
        /// <summary>
        /// Qualified leads for a specific channel
        /// </summary>
        [Description("Qualified Leads")]
        QualifiedCount = 1,
        /// <summary>
        /// Customers created with revenue > 0
        /// </summary>
        [Description("Customers Created")]
        CustomerCount = 2,
        /// <summary>
        /// Conversion rate
        /// </summary>
        [Description("Conversion Rate")]
        ConversionRate = 3,
        /// <summary>
        /// Revenue
        /// </summary>
        [Description("Revenue")]
        Revenue = 4,
        /// <summary>
        /// Number of Deals
        /// </summary>
        [Description("Number of Deals")]
        DealCount = 5,
        /// <summary>
        /// Avg Deal Amount
        /// </summary>
        [Description("Avg Deal Amount")]
        AverageDealAmount = 6,
        /// <summary>
        /// Avg Days To Purchase
        /// </summary>
        [Description("Avg Days To Purchase")]
        AverageTimeToPurchase = 7,
    }

    /// <summary>
    /// Top level measure of specific account metrics
    /// </summary>
    public class LeadChannelMetricRecord
    {
        /// <summary>
        /// Used in ORM or subclassing scenarios.
        /// </summary>
        protected LeadChannelMetricRecord()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MrrMetricRecord"/> class.
        /// </summary>
        public LeadChannelMetricRecord(LeadChannelMetricName metricName)
        {
            this.MetricName = metricName;
        }

        /// <summary>
        /// Description of metric
        /// </summary>
        public LeadChannelMetricName MetricName { get; protected set; }

        /// <summary>
        /// Returns plain text description of MetricName property
        /// </summary>
        public String MetricNameDescription => this.MetricName.GetDescription();

        /// <summary>
        /// All activity for the current day starting at the most recent midnight
        /// </summary>
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime Key { get; set; }

        /// <summary>
        /// All activity for the current day starting at the most recent midnight
        /// </summary>
        [DefaultValue(0)]
        public Decimal? Value { get; set; }
    }
}