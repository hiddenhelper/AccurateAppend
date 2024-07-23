using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DomainModel.Queries
{
    /// <summary>
    /// Report containing top level UserStatus and jive metrics.
    /// </summary>
    public interface IAdminUserActivityMetricQuery
    {
        /// <summary>
        /// Intializes a new queryable against the deal metrics for the indicated application.
        /// </summary>
        /// <param name="applicationId">The identifier of the application to query metrics for.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> used to signal that an operation should be canceled.</param>
        /// <returns>A new queryable that can be further customized.</returns>
        Task<IEnumerable<AdminUserActivityMetricRecord>> Query(Guid applicationId, CancellationToken cancellation = default(CancellationToken));
    }

    /// <summary>
    /// Top level measure of specific admin user metrics
    /// </summary>
    public class AdminUserActivityMetricRecord
    {
        /// <summary>
        /// Used in ORM or subclassing scenarios.
        /// </summary>
        protected AdminUserActivityMetricRecord()
        {
        }

        /// <summary>
        /// Date of metric
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Hour of metric
        /// </summary>
        public int Hour { get; set; }

        /// <summary>
        /// Number of unique callers
        /// </summary>
        public int Callers { get; set; }

        /// <summary>
        /// Number of unique inbound phone numbers
        /// </summary>
        public int Inbound { get; set; }

        /// <summary>
        /// Number of unique outbound phone numbers
        /// </summary>
        public int Outbound { get; set; }

        /// <summary>
        /// Number of voicemails left
        /// </summary>
        public int Voicemail { get; set; }

        /// <summary>
        /// Admin logon events
        /// </summary>
        public int LoginEvent { get; set; }

        /// <summary>
        /// Number of unique leads touched
        /// </summary>
        public int LeadsTouched { get; set; }

        /// <summary>
        /// Number of unique customers touched
        /// </summary>
        public int CustomersTouched { get; set; }

        /// <summary>
        /// Number of unique deals touched
        /// </summary>
        public int DealsTouched { get; set; }
    }
}