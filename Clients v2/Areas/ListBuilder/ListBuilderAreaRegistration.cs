using System;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients.Areas.ListBuilder
{
    /// <summary>
    /// Registers the "ListBuilder" area.
    /// </summary>
    public class ListBuilderAreaRegistration : AreaRegistration 
    {
        /// <summary>
        /// Gets the name of the area to register.
        /// </summary>
        /// <returns>
        /// The name of the area to register.
        /// </returns>
        public override String AreaName => "ListBuilder";

        /// <summary>
        /// Registers an area in an ASP.NET MVC application using the specified area's context information.
        /// </summary>
        /// <param name="context">Encapsulates the information that is required in order to register the area.</param>
        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ListBuilder_default",
                "ListBuilder/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}