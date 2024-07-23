using System;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients.Areas.JobProcessing
{
    /// <summary>
    /// Registers the "JobProcessing" area.
    /// </summary>
    public class JobProcessingAreaRegistration : AreaRegistration 
    {
        /// <summary>
        /// Gets the name of the area to register.
        /// </summary>
        /// <returns>
        /// The name of the area to register.
        /// </returns>
        public override String AreaName => "JobProcessing";

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