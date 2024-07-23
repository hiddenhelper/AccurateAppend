using System;
using System.Web;
using AccurateAppend.Core.Definitions;
using Castle.Windsor;
using EventLogger;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Http Application for the Admin application.
    /// </summary>
    public class MvcApplication : HttpApplication
    {
        /// <summary>
        /// Holds the system wide container instance that is used for component initialization.
        /// </summary>
        private static IWindsorContainer HostContainerInstance;

        /// <summary>
        /// Event sink for the <see cref="HttpApplication.Error"/> event.
        /// </summary>
        protected void Application_Error()
        {
            var ctx = HttpContext.Current;
            var ex = ctx.Server.GetLastError();

            Logger.LogEvent(ex, Severity.Low, Core.Definitions.Application.AccurateAppend_Admin, ctx.Request.UserHostAddress, "Exception encountered on Admin");
            ctx.Response.Clear();
        }

        /// <summary>
        /// Event sink for the system start event.
        /// </summary>
        protected void Application_Start()
        {
            var bootstrapper = new Bootstrapper();
            bootstrapper.Initialized += (s, e) => Logger.LogEvent("Admin completed startup", Severity.None, Core.Definitions.Application.AccurateAppend_Admin);
            HostContainerInstance = bootstrapper.Run();

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                lock (Logger.SyncLock)
                {
                    Logger.Loggers.Clear();
                }
            }
#endif
            Logger.GlobalOverride(Core.Definitions.Application.AccurateAppend_Admin);
        }

        /// <summary>
        /// Event sink for the <see cref="HttpApplication.PostAcquireRequestState"/> event.
        /// </summary>
        protected void Application_PostAcquireRequestState(Object sender, EventArgs e)
        {
            Security.ContextExtensions.ConfigureClaimsForInteractiveUser(HttpContext.Current);
        }

        /// <summary>
        /// Provides a pointer to the application wide base container.
        /// </summary>
        public static IWindsorContainer Container => HostContainerInstance;
    }
}