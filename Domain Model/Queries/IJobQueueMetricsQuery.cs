using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DomainModel.Queries
{
    /// <summary>
    /// A query component that can provides access to <see cref="JobQueueMetric"/> queries.
    /// </summary>
    public interface IJobQueueMetricsQuery
    {
        /// <summary>
        /// Crafts a reqult for <see cref="JobQueueMetric"/> entities for the indicated user that have been processed during the indicated time frame.
        /// </summary>
        Task<IEnumerable<JobQueueMetric>> Query(Guid applicationId, DateTime start, DateTime end, CancellationToken cancellation);
    }

    public class JobQueueMetric
    {
        public Int32 Year { get; set; }
        public Int32 Month { get; set; }
        public Int32 Day { get; set; }
        public Int32 Hour { get; set; }
        public Int32 FtpCount { get; set; }
        public Int32 FtpUsers { get; set; }
        public Int32 FtpRecords { get; set; }
        public Int32 EmailCount { get; set; }
        public Int32 EmailUsers { get; set; }
        public Int32 EmailRecords { get; set; }
        public Int32 AdminCount { get; set; }
        public Int32 AdminUsers { get; set; }
        public Int32 AdminRecords { get; set; }
        public Int32 NationBuilderCount { get; set; }
        public Int32 NationBuilderUsers { get; set; }
        public Int32 NationBuilderRecords { get; set; }
        public Int32 ClientCount { get; set; }
        public Int32 ClientUsers { get; set; }
        public Int32 ClientRecords { get; set; }
        public Int32 ListbuilderCount { get; set; }
        public Int32 ListbuilderUsers { get; set; }
        public Int32 ListbuilderRecords { get; set; }
    }
}
