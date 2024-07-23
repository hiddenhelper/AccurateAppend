using System;
using System.Diagnostics;
using System.Web;
using AccurateAppend.Core.Definitions;
using Castle.Windsor;
using EventLogger;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Http Application for the Clients application.
    /// </summary>
    /// <remarks>
    /// Responsible for holding reference to the system level container and initializing the configuration at start.
    /// </remarks>
    public class MvcApplication : HttpApplication, IContainerAccessor
    {
        #region Fields

        /// <summary>
        /// Holds the system wide container instance that is used for component initialization.
        /// </summary>
        private static IWindsorContainer HostContainerInstance;

        #endregion

        #region Event Sinks

        /// <summary>
        /// Event raised when the application starts.
        /// </summary>
        protected void Application_Start(Object sender, EventArgs e)
        {
            HostContainerInstance = Bootstrapper.Configure((s, args) =>
            {
                Logger.LogEvent("Clients completed startup", Severity.None, Core.Definitions.Application.Clients);
            });
        }

        /// <summary>
        /// Event sink for the <see cref="HttpApplication.PostAcquireRequestState"/> event.
        /// </summary>
        protected void Application_PostAcquireRequestState(Object sender, EventArgs e)
        {
            Security.RequestContextHelper.ConfigureClaimsForInteractiveUser(HttpContext.Current);
        }

        /// <summary>
        /// Event sink for the <see cref="HttpApplication.Error"/> event.
        /// </summary>
        protected void Application_Error()
        {
            var ctx = HttpContext.Current;
            var ex = ctx.Server.GetLastError();

            if (Debugger.IsAttached) Debugger.Break();

            Log(ex);
            ctx.Response.Clear();
        }

        #endregion

        #region IContainerAccessor Members

        ///  <inheritdoc />
        public IWindsorContainer Container => HostContainerInstance;

        #endregion

        #region Global Exception Logging

        /// <summary>
        /// Standardized logging point.
        /// </summary>
        public static void Log(Exception ex, String description = "Exception encountered on Clients", Severity severity = Severity.Low)
        {
            Logger.LogEvent(ex, TraceEventType.Error, severity, description);
        }
        #endregion
    }
}