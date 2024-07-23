using System;
using System.Linq;

namespace AccurateAppend.Sales.ReadModel.Queries
{
    /// <summary>
    /// A query component that can provides access to <see cref="DealNoteView"/> queries.
    /// </summary>
    public interface IDealNotesQuery
    {
        /// <summary>
        /// Crafts a queryable for <see cref="DealNoteView"/> entities that are related to a specific deal.
        /// </summary>
        IQueryable<DealNoteView> ForDeal(Int32 dealId);
    }
}