using System.Diagnostics;
using System.Web;
using AccurateAppend.Core.Definitions;
using EventLogger;

namespace AccurateAppend.Websites.Storage
{
    /// <summary>
    /// Http Application for the Storage application.
    /// </summary>
    public class MvcApplication : HttpApplication
    {
        #region Event Sinks

        /// <summary>
        /// Event sink for the Error event.
        /// </summary>
        protected void Application_Error()
        {
            var ctx = HttpContext.Current;
            var ex = ctx.Server.GetLastError();

            if (Debugger.IsAttached) Debugger.Break();

            Logger.LogEvent(ex, TraceEventType.Error, Severity.Low, "Clients", ctx.Request.UserHostAddress, "Exception encountered on Azure");
            ctx.Response.Clear();
        }

        /// <summary>
        /// Event sink for the Start event.
        /// </summary>
        protected void Application_Start()
        {
            (new Bootstrapper()).Run();
        }

        #endregion
    }
}
