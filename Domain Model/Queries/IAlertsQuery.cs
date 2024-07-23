using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Security;

namespace DomainModel.Queries
{
    /// <summary>
    /// A query component that can provides access to <see cref="UserAlert"/> queries.
    /// </summary>
    public interface IAlertsQuery
    {
        /// <summary>
        /// Crafts a query that can acquire active <see cref="UserAlert"/>.
        /// </summary>
        /// <param name="userId">The identifier of the <see cref="User"/> that the alerts are for.</param>
        /// <param name="topic">The optional topic namespace for desired alerts.</param>
        /// <returns>A new queryable for the data that can be further customized.</returns>
        IQueryable<UserAlert> Active(Guid userId, String topic = null);

        /// <summary>
        /// Crafts a query that can acquire active <see cref="UserAlert"/>.
        /// </summary>
        /// <param name="userId">The identifier of the <see cref="User"/> that the alerts are for.</param>
        /// <param name="cancellation">The <see cref="CancellationToken"/> that should be monitored for cancealltion requests.</param>
        /// <param name="topic">The optional topic namespace for desired alerts.</param>
        /// <returns>A count for the current outstanding active alerts for the indicated user and topic.</returns>
        Task<Int32> Count(Guid userId, CancellationToken cancellation, String topic = null);
    }
}