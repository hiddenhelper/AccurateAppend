using System;
using System.Linq;

namespace DomainModel.Queries
{
    /// <summary>
    /// A query component that can provides access to <see cref="ReadModel.LeadView"/> queries.
    /// </summary>
    public interface ILeadsViewQuery
    {
        /// <summary>
        /// Crafts a queryable for <see cref="ReadModel.LeadView"/> entities that have been active during the indicated time frame.
        /// </summary>
        IQueryable<ReadModel.LeadView> ActiveDuring(Guid applicationId, DateTime startOn, DateTime endBy);

        /// <summary>
        /// Performs a fuzzy leads search using the indicated term.
        /// </summary>
        /// <param name="term">The value to search lead information with.</param>
        IQueryable<ReadModel.LeadView> Search(String term);
    }
}
