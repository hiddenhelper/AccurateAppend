using System;
using System.Linq;
using DomainModel.ReadModel;

namespace DomainModel.Queries
{
    /// <summary>
    /// A query component that can provides access to <see cref="ChargeEvent"/> queries.
    /// </summary>
    public interface IChargeEventsQuery
    {
        /// <summary>
        /// Creates a queryable for the indicated application that occured on or later than a start date
        /// and before the end date.
        /// </summary>
        IQueryable<ChargeEvent> ForApplication(Guid applicationId, DateTime startOn, DateTime endBy);

        /// <summary>
        /// Creates a queryable for the indicated charge event instance.
        /// </summary>
        IQueryable<ChargeEvent> ForId(Int32 id);

        /// <summary>
        /// Creates a queryable for the indicated charge event on a deal.
        /// </summary>
        IQueryable<ChargeEvent> ForDeal(Int32 id);
    }
}
