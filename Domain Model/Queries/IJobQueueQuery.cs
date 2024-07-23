using System;
using System.Linq;


namespace DomainModel.Queries
{
    /// <summary>
    /// A query component that can provides access to <see cref="ReadModel.JobQueueSummary"/> queries.
    /// </summary>
    public interface IJobQueueQuery
    {
        /// <summary>
        /// Crafts a queryable for <see cref="ReadModel.JobQueueView"/> entities for jobs that are processing.
        /// </summary>
        IQueryable<ReadModel.JobQueueView> InProgress(Guid applicationId);

        /// <summary>
        /// Crafts a queryable for <see cref="ReadModel.JobQueueView"/> entities for jobs that have been completed during the indicated time frame.
        /// </summary>
        IQueryable<ReadModel.JobQueueView> CompletedDuring(Guid userId, DateTime startOn, DateTime endBy);

        /// <summary>
        /// Crafts a queryable for <see cref="ReadModel.JobQueueView"/> entities for jobs that have been completed during the indicated time frame.
        /// </summary>
        IQueryable<ReadModel.JobQueueView> CompletedDuring(String userName, DateTime startOn, DateTime endBy);

        /// <summary>
        /// Crafts a queryable for <see cref="ReadModel.JobQueueSummary"/> entities for jobs that have been completed during the indicated time frame.
        /// </summary>
        IQueryable<ReadModel.JobQueueSummary> CompletedSummary(Guid applicationId, DateTime startOn, DateTime endBy);

        /// <summary>
        /// Crafts a queryable for <see cref="ReadModel.JobQueueSummary"/> entities for jobs that have been completed during the indicated time frame.
        /// </summary>
        IQueryable<ReadModel.JobQueueView> SearchDuring(Guid applicationId, string searchTerm, DateTime startOn, DateTime endBy);
    }
}
