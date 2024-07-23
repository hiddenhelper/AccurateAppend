using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Data;
using AccurateAppend.Security;

namespace DomainModel.Queries
{
    /// <summary>
    /// Default implementation of the <see cref="IAlertsQuery"/> interface.
    /// </summary>
    public class AlertsQuery : IAlertsQuery
    {
        #region Fields

        private readonly ISessionContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AlertsQuery"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> used to provide data access.</param>
        public AlertsQuery(ISessionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Crafts a query that can acquire active <see cref="UserAlert"/>.
        /// </summary>
        /// <param name="userId">The identifier of the <see cref="User"/> that the alerts are for.</param>
        /// <param name="topic">The optional topic namespace for desired alerts.</param>
        /// <returns>A new queryable for the data that can be further customized.</returns>
        public virtual IQueryable<UserAlert> Active(Guid userId, String topic = null)
        {
            var alerts = this.context.SetOf<UserAlert>()
                   .Where(a => a.Status == AlertStatus.Issued && a.IssuedOn >= DateTime.UtcNow && (a.ValidUntil == null || a.ValidUntil > DateTime.UtcNow))
                   .Where(a => a.Subject.Id == userId);

            if (!String.IsNullOrWhiteSpace(topic)) alerts = alerts.Where(a => a.Namespace == topic);

            return alerts.OrderByDescending(a => a.IssuedOn);
        }

        /// <summary>
        /// Crafts a query that can acquire active <see cref="UserAlert"/>.
        /// </summary>
        /// <param name="userId">The identifier of the <see cref="User"/> that the alerts are for.</param>
        /// <param name="cancellation">The <see cref="CancellationToken"/> that should be monitored for cancealltion requests.</param>
        /// <param name="topic">The optional topic namespace for desired alerts.</param>
        /// <returns>A count for the current outstanding active alerts for the indicated user and topic.</returns>
        public virtual Task<Int32> Count(Guid userId, CancellationToken cancellation, String topic = null)
        {
            return this.Active(userId, topic).CountAsync(cancellation);
        }

        #endregion
    }
}
