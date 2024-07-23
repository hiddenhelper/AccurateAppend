using System;
using System.Linq;

namespace AccurateAppend.Sales.ReadModel.Queries
{
    /// <summary>
    /// A query component that can provides access to <see cref="DealView"/> queries.
    /// </summary>
    public interface IDealsViewActiveDuringQuery
    {
        /// <summary>
        /// Crafts a queryable for <see cref="DealView"/> entities that have been active during the indicated time frame.
        /// </summary>
        IQueryable<DealView> ActiveDuring(DateTime startOn, DateTime endBy);
    }
}
