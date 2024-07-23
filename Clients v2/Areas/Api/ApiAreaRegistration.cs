using System;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients.Areas.Api
{
    /// <summary>
    /// Registers the "Api" area.
    /// </summary>
    public class ApiAreaRegistration : AreaRegistration
    {
        /// <inheritdoc />
        public override String AreaName => "Api";

        /// <inheritdoc />
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Api_default",
                "Api",
                new { controller = "Home", action = "Index" }
            );

            context.MapRoute(
                "Api_standard",
                "Api/{controller}/{action}",
                new { action = "Index" }
            );
        }
    }
}
