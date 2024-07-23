using AccurateAppend.Accounting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core;

namespace DomainModel.Queries
{
    /// <summary>
    /// Report containing top level Deal and UserDetail metrics.
    /// </summary>
    public interface IDealMetricQuery
    {
        /// <summary>
        /// Intializes a new queryable against the deal metrics for the indicated application.
        /// </summary>
        /// <param name="applicationId">The identifier of the application to query metrics for.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> used to signal that an operation should be canceled.</param>
        /// <returns>A new queryable that can be further customized.</returns>
        Task<IEnumerable<DealMetricRecord>> Query(Guid applicationId, CancellationToken cancellation);
    }

    /// <summary>
    /// Top level measure of specific account metrics
    /// </summary>
    public class DealMetricRecord
    {
        /// <summary>
        /// Used in ORM or subclassing scenarios.
        /// </summary>
        protected DealMetricRecord()
        {
        }

        public DealMetricRecord(LeadSource leadSource)
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
        /// Description of metric
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// All activity for the current day starting at the most recent midnight
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// All activity for the current day starting at the most recent midnight
        /// </summary>
        public Decimal Amount { get; set; }

        /// <summary>
        /// All activity for the prior day ending at the most recent midnight
        /// </summary>
        public int DealId { get; set; }

        /// <summary>
        /// All activity for the prior day ending at the most recent midnight
        /// </summary>
        public DateTime DateDealCreated { get; set; }

        /// <summary>
        /// All activity for the prior 7 days ending at the most recent midnight
        /// </summary>
        public DateTime DateAccountCreated { get; set; }

        /// <summary>
        /// All activity for the current month
        /// </summary>
        public string Category { get; set; }
    }
}