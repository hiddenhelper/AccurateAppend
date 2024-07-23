using System;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using DomainModel.Commands;
using DomainModel.Queries;

namespace AccurateAppend.Websites.Admin.Areas.Operations.Alerts
{
    /// <summary>
    /// Controller for managing alerts for the interactive account.
    /// </summary>
    [Authorize()]
    public class AlertsController : Controller
    {
        #region Fields

        private readonly IAlertsQuery query;
        private readonly IAcknowledgeAlertCommand command;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AlertsController"/> class.
        /// </summary>
        /// <param name="query">The <see cref="IAlertsQuery"/> used to provide data access.</param>
        /// <param name="command">The <see cref="IAcknowledgeAlertCommand"/> used to provide data logic.</param>
        public AlertsController(IAlertsQuery query, IAcknowledgeAlertCommand command)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            Contract.EndContractBlock();

            this.query = query;
            this.command = command;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Acquires any alerts from the system to the currently logged in user.
        /// </summary>
        [HttpGet()]
        public virtual async Task<ActionResult> Current(String @namespace, CancellationToken cancellation)
        {
            var userId = this.User.Identity.GetIdentifier();

            var alerts = this.query.Active(userId, @namespace)
                .Take(5)
                .OrderByDescending(a => a.IssuedOn);

            var data = await alerts.Select(a => new { a.Id, a.Message, a.ValidUntil, a.Namespace }).ToArrayAsync(cancellation);
            return this.Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Marks the indicated alert as being acknowledged and not to be shown anymore.
        /// </summary>
        [HttpPut()]
        public virtual async Task<ActionResult> Acknowledge(Int32 id, CancellationToken cancellation)
        {
            try
            {
                var userId = this.User.Identity.GetIdentifier();
                await this.command.Acknowledge(id, userId, cancellation);
            }
            catch (Exception ex)
            {
                EventLogger.Logger.LogEvent(ex, Severity.Medium, Application.AccurateAppend_Admin, null, $"Failure to Acknowledge alert:{id}");
                return this.Json(new { Status = HttpStatusCode.InternalServerError, Message = "Unable to Acknowledge alert due to server failure" });
            }

            return this.Json(new { Status = HttpStatusCode.OK, Message = "Success" });
        }

        /// <summary>
        /// Acquires a count of any alerts from the system to the currently logged in user.
        /// </summary>
        [HttpGet()]
        public virtual async Task<ActionResult> Count(CancellationToken cancellation)
        {
            var userId = this.User.Identity.GetIdentifier();
            var count = await this.query.Count(userId, cancellation);

            return this.Json(new {Count = count}, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}