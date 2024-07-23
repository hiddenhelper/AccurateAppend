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
    public interface IMrrMetricQuery
    {
        /// <summary>
        /// Executes a new query against the monthly recurring revenue metrics for the indicated application.
        /// </summary>
        /// <param name="applicationId">The identifier of the application to query metrics for.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> used to signal that an operation should be canceled.</param>
        /// <returns>A new queryable that can be further customized.</returns>
        Task<IEnumerable<MrrMetricRecord>> Query(Guid applicationId, CancellationToken cancellation = default(CancellationToken));
    }

    /// <summary>
    /// Mrr Metrics
    /// </summary>
    [Serializable()]
    public enum MrrMetricName
    {
        /// <summary>
        /// Revenue from subscriptions
        /// </summary>
        [Description("Gross Revenue")]
        GrossRevenue = 1,
        /// <summary>
        /// Revenue from subscriptions
        /// </summary>
        [Description("MRR - Subscription")]
        MrrSubscription = 2,
        /// <summary>
        /// Revenue from usage and overage
        /// </summary>
        [Description("MRR - Usage and Overage")]
        MrrUsageAndOverage = 3,
        /// <summary>
        /// Revenue from new subscriptions
        /// </summary>
        [Description("Expansion")]
        NewMrrSubscription = 4,
        /// <summary>
        /// Usage and overage from bew subscriptions
        /// </summary>
        [Description("MRR - Expansion - Usage and Overage")]
        NewMrrUsageAndOverage = 5,
        /// <summary>
        /// Revenue from cancelled subscriptions
        /// </summary>
        [Description("MRR - Churned Subscription")]
        ChurnedMrrSubscription = 6,
        /// <summary>
        /// Usage and overage from cancelled subscriptions
        /// </summary>
        [Description("MRR - Churned - Usage and Overage")]
        ChurnedMrrUsageAndOverage = 7
    }

    /// <summary>
    /// Top level measure of specific account metrics
    /// </summary>
    public class MrrMetricRecord
    {
        /// <summary>
        /// Used in ORM or subclassing scenarios.
        /// </summary>
        protected MrrMetricRecord()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MrrMetricRecord"/> class.
        /// </summary>
        public MrrMetricRecord(MrrMetricName metricName)
        {
            this.MetricName = metricName;
        }

        /// <summary>
        /// Description of metric
        /// </summary>
        public MrrMetricName MetricName { get; protected set; }

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
        public Decimal? Amount { get; set; }
    }
}