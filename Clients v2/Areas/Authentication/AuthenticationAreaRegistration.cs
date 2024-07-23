using System;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients.Areas.Authentication
{
    /// <summary>
    /// Registers the "Authentication" area.
    /// </summary>
    public class AuthenticationAreaRegistration : AreaRegistration
    {
        /// <summary>
        /// Gets the name of the area to register.
        /// </summary>
        /// <returns>
        /// The name of the area to register.
        /// </returns>
        public override String AreaName => "Authentication";

        /// <summary>
        /// Registers an area in an ASP.NET MVC application using the specified area's context information.
        /// </summary>
        /// <param name="context">Encapsulates the information that is required in order to register the area.</param>
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Authentication_default",
                "Authentication",
                new { controller = "Direct", action = "Login" }
            );

            context.MapRoute(
                "Authentication_Logoff",
                "Authentication/LogOff/{action}",
                new { controller = "LogOff", action = "Index" }
            );

            context.MapRoute(
                "Authentication_Standard",
                "Authentication/{controller}/{action}",
                new { action = "Login" }
            );
        }
    }
}
