using System;
using System.Diagnostics.Contracts;
using System.Web;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Storage
{
    /// <summary>
    /// Enable CORS pre-flight checks on a MVC application and specific Action methods when applied.
    /// The default implementation is to not restrict the list to a static known set of sources but
    /// to echo back the origin.
    /// </summary>
    public class CorsHandlerAttribute : ActionFilterAttribute, IHttpModule, IDisposable
    {
        #region Fields

        private Boolean isDisposed;
        private Boolean isInitialized;

        #endregion

        #region Destructor

        /// <summary>
        /// Destructor.
        /// </summary>
        ~CorsHandlerAttribute()
        {
            this.Dispose(false);
        }

        #endregion

        #region IHttpModule Members

        /// <summary>Initializes a module and prepares it to handle requests.</summary>
        /// <param name="context">An <see cref="T:System.Web.HttpApplication" /> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application </param>
        public virtual void Init(HttpApplication context)
        {
            Contract.Ensures(this.isInitialized);
            Contract.EndContractBlock();

            if (this.isInitialized) return;

            context.BeginRequest += this.OnBeginRequest;

            this.isInitialized = true;
        }

        /// <summary>
        /// Hook method that begins the CORS handler logic.
        /// </summary>
        /// <param name="sender">The source of the event. This MUST always be an <see cref="HttpApplication"/> object.</param>
        /// <param name="e">The <see cref="EventArgs"/> describing the event. This is not used.</param>
        protected virtual void OnBeginRequest(Object sender, EventArgs e)
        {
            var context = sender as HttpApplication;
            if (context == null) return;
            if (context.Request.HttpMethod != "OPTIONS") return;

            this.AppendHeaders(new HttpContextWrapper(context.Context));

            context.Response.Flush();
            context.CompleteRequest();
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule" />.
        /// </summary>
        /// <remarks>
        /// This method doubles as the <see cref="IHttpModule.Dispose"/> and <see cref="IDisposable.Dispose"/> method. The behavior
        /// is fixed in either case. Perform your clean up work by overriding the <see cref="Dispose(Boolean)"/> method instead.
        /// </remarks>
        public void Dispose()
        {
            if (this.isDisposed) return;

            this.Dispose(true);
            this.isDisposed = true;

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs the actual tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="isDisposing">Indicates whether the current call is the result of an explicit call to <see cref="IDisposable.Dispose"/>.</param>
        protected virtual void Dispose(Boolean isDisposing)
        {
        }

        #endregion

        #region Overrides

        /// <summary>Called by the ASP.NET MVC framework after the action result executes.</summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);

            this.AppendHeaders(filterContext.HttpContext);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Performs the actual work required to 
        /// </summary>
        /// <param name="context"></param>
        protected virtual void AppendHeaders(HttpContextBase context)
        {
            var origin = context.Request.Headers["Origin"];
            var allowOrigin = !string.IsNullOrWhiteSpace(origin) ? origin : "*";
            context.Response.AddHeader("Access-Control-Allow-Origin", allowOrigin);
            context.Response.AddHeader("Access-Control-Allow-Headers", "*");
            context.Response.AddHeader("Access-Control-Allow-Credentials", "true");
        }

        /// <summary>
        /// Registers the <see cref="CorsHandlerAttribute"/> as a  module to enable CORS pre-flight requests across the entire site.
        /// This method MUST be called as part and ASP.Net PreApplicationStartMethod usage. If this is unavailable, the module may
        /// be statically configured in the "system.webServer/httpModules" collection in the config file instead.
        /// </summary>
        public static void EnablePreflightChecks()
        {
            //DynamicModuleUtility.RegisterModule(typeof(CorsHandlerAttribute));
        }

        #endregion
    }
}