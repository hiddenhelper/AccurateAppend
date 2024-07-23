using System;
using System.Linq;
using DomainModel.ReadModel;

namespace DomainModel.Queries
{
    /// <summary>
    /// A query component that can provides access to <see cref="ClientView"/> queries.
    /// </summary>
    public interface IClientsViewQuery
    {
        /// <summary>
        /// Crafts a queryable for <see cref="ClientView"/> entities that have been active during the indicated time frame.
        /// </summary>
        IQueryable<ClientView> ActiveDuring(DateTime startOn, DateTime endBy);

        /// <summary>
        /// Performs a fuzzy clients search using the indicated term.
        /// </summary>
        /// <param name="term">The value to search client information with.</param>
        IQueryable<ClientView> Search(String term);
    }
}
