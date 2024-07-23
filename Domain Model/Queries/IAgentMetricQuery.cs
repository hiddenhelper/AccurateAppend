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
    /// Report containing top level <see cref="AccurateAppend.Accounting.Deal"/> metrics by Agent.
    /// </summary>
    public interface IAgentMetricQuery
    {
        /// <summary>
        /// Initializes a new queryable against the monthly recurring revenue metrics for the indicated application.
        /// </summary>
        /// <param name="applicationId">The identifier of the application to query metrics for.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> used to signal that an operation should be canceled.</param>
        /// <returns>A new queryable that can be further customized.</returns>
        Task<IEnumerable<AgentMetricRecord>> Query(Guid applicationId, CancellationToken cancellation = default(CancellationToken));
    }

    /// <summary>
    /// Mrr Metrics
    /// </summary>
    [Serializable()]
    public enum AgentMetricName
    {
        /// <summary>
        /// Gross revenue
        /// </summary>
        [Description("Gross Revenue")]
        Gross = 1,

        /// <summary>
        /// Revenue from subscriptions
        /// </summary>
        [Description("House")]
        System = 2,

        /// <summary>
        /// Revenue from Steve accounts
        /// </summary>
        [Description("Steve McGavran")]
        Steve = 3,

        /// <summary>
        /// Revenue from Chris accounts
        /// </summary>
        [Description("Chris Nichols")]
        Chris = 4,

        /// <summary>
        /// Revenue from Andy accounts
        /// </summary>
        [Description("Andy Anthony")]
        Andy = 5,

        /// <summary>
        /// Revenue from Max accounts
        /// </summary>
        [Description("Max Carone")]
        Max = 6,

        /// <summary>
        /// Other account revenue
        /// </summary>
        [Description("All Other")]
        Other = 7
    }

    /// <summary>
    /// Top level measure of specific account metrics
    /// </summary>
    public class AgentMetricRecord
    {
        /// <summary>
        /// Used in ORM or subclassing scenarios.
        /// </summary>
        protected AgentMetricRecord()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MrrMetricRecord"/> class.
        /// </summary>
        public AgentMetricRecord(AgentMetricName metricName)
        {
            this.MetricName = metricName;
        }

        /// <summary>
        /// Description of metric
        /// </summary>
        public AgentMetricName MetricName { get; protected set; }

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