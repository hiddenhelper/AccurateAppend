using System;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Admin.Areas.JobProcessing
{
    /// <summary>
    /// Auto-discovery registration component for the "JobProcessing" area routes.
    /// </summary>
    public class JobAreaRegistration : AreaRegistration
    {
        /// <summary>
        /// Gets the name of the area to register.
        /// </summary>
        /// <returns>
        /// The name of the area to register.
        /// </returns>
        public override String AreaName { get; } = "JobProcessing";

        /// <summary>
        /// Registers an area in an ASP.NET MVC application using the specified area's context information.
        /// </summary>
        /// <param name="context">Encapsulates the information that is required in order to register the area.</param>
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "JobProcessing_default",
                "JobProcessing/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
