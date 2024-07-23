using System;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder
{
    /// <summary>
    /// Registers the "NationBuilder" area.
    /// </summary>
    public class NationBuilderAreaRegistration : AreaRegistration
    {
        /// <summary>
        /// Gets the name of the area to register.
        /// </summary>
        /// <returns>
        /// The name of the area to register.
        /// </returns>
        public override String AreaName => "NationBuilder";

        /// <summary>
        /// Registers an area in an ASP.NET MVC application using the specified area's context information.
        /// </summary>
        /// <param name="context">Encapsulates the information that is required in order to register the area.</param>
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "NationBuilder_default",
                "NationBuilder",
                new { controller = "Signup", action = "Index" }
            );

            context.MapRoute(
                "NationBuilder_Standard",
                "NationBuilder/{controller}/{action}",
                new { action = "Index" }
            );
        }
    }
}
