using System;
using System.Diagnostics.Contracts;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Websites.Clients.Areas.NationBuilder.ChangeAccess.Messages;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.ChangeAccess
{
    /// <summary>
    /// Controller for dispatching toggle activation commands for a NationBuilder registration.
    /// </summary>
    [Authorize()]
    public class ChangeAccessController : Controller
    {
        #region Fields

        private readonly IMessageSession bus;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeAccessController" /> class.
        /// </summary>
        /// <param name="bus">The <see cref="IMessageSession" /> component providing access to the bus.</param>
        public ChangeAccessController(IMessageSession bus)
        {
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            Contract.EndContractBlock();

            this.bus = bus;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Starts the process of deactivating a specified Nation.
        /// </summary>
        public virtual async Task<ActionResult> Deactivate(Int32 id, Guid requestId, CancellationToken cancellation)
        {
            try
            {
                var command = new ToggleNationAccessCommand();
                command.NationId = id;
                command.Enable = false;
                command.RequestId = requestId;

                await this.bus.Send(command);

                return this.Json(new { Success = HttpStatusCode.Accepted, Message = "Request processing" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                EventLogger.Logger.LogEvent(ex, Severity.Medium, Application.Clients, this.Request.UserHostAddress, $"Error deactivating nation {id}");

                return this.Json(new { Success = HttpStatusCode.InternalServerError, Message = "Error deactivating Nation" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Starts the process of reactivating a specified Nation.
        /// </summary>
        public virtual async Task<ActionResult> Reactivate(Int32 id, Guid requestId, CancellationToken cancellation)
        {
            try
            {
                var command = new ToggleNationAccessCommand();
                command.NationId = id;
                command.Enable = true;
                command.RequestId = requestId;
                
                await this.bus.Send(command);

                return this.Json(new { Success = HttpStatusCode.Accepted, Message = "Request processing" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                EventLogger.Logger.LogEvent(ex, Severity.Medium, Application.Clients, this.Request.UserHostAddress, $"Error reactivating nation {id}");

                return this.Json(new { Success = HttpStatusCode.InternalServerError, Message = "Error reactivating Nation" }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}