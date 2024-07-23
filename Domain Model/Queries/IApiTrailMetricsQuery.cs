using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DomainModel.Queries
{
    /// <summary>
    /// Report containing top level Deal  metrics.
    /// </summary>
    public interface IApiTrailMetricsQuery
    {
        /// <summary>
        /// Initializes a new queryable against the api trail metrics for the indicated user.
        /// </summary>
        /// <remarks>
        ///     Returns transaction count grouped method
        /// </remarks>
        /// <param name="userid">The identifier of of the user to query metrics for.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> used to signal that an operation should be canceled.</param>
        /// <returns>A new queryable that can be further customized.</returns>
        Task<IEnumerable<ApiTrailMetricsRecord>> QueryMethodCallsCounts(Guid userid, CancellationToken cancellation);

        /// <summary>
        /// Initializes a new queryable against the api trail metrics for the indicated user.
        /// </summary>
        /// <remarks>
        ///     Returns Operation count grouped method
        /// </remarks>
        /// <param name="userid">The identifier of of the user to query metrics for.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> used to signal that an operation should be canceled.</param>
        /// <returns>A new queryable that can be further customized.</returns>
        Task<IEnumerable<ApiTrailMetricsRecord>> QueryOperationCounts(Guid userid, CancellationToken cancellation);
    }

    /// <summary>
    /// Top level measure of specific account metrics
    /// </summary>
    public class ApiTrailMetricsRecord
    {
        /// <summary>
        /// Used in ORM or subclassing scenarios.
        /// </summary>
        protected ApiTrailMetricsRecord()
        {
        }
        
        /// <summary>
        /// Description of metric
        /// </summary>
        public string Description { get; protected set; }

        /// <summary>
        /// Number of calls to method
        /// </summary>
        public Int32 Count { get; set; }

        /// <summary>
        /// All activity for the current day starting at the most recent midnight
        /// </summary>
        public Int32 Today { get; set; }

        /// <summary>
        /// All activity for the prior day ending at the most recent midnight
        /// </summary>
        public Int32 Yesterday { get; set; }
       
        /// <summary>
        /// All activity for the last 7 days
        /// </summary>
        public Int32 Last7 { get; set; }

        /// <summary>
        /// All activity for the last 30 days
        /// </summary>
        public Int32 Last30 { get; set; }
    }
}