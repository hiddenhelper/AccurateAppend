using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace AccurateAppend.Websites.Admin.Areas.ListBuilder
{
    /// <summary>
    /// Auto-discovery registration component for the "ListBuilder" area routes.
    /// </summary>
    public class ListBuilderAreaRegistration : AreaRegistration 
    {
        /// <summary>
        /// Gets the name of the area to register.
        /// </summary>
        /// <returns>
        /// The name of the area to register.
        /// </returns>
        public override String AreaName { get; } = "ListBuilder";

        /// <summary>
        /// Registers an area in an ASP.NET MVC application using the specified area's context information.
        /// </summary>
        /// <param name="context">Encapsulates the information that is required in order to register the area.</param>
        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.Routes.Add(new Route("ListBuilder/Scripts/{filename}.{js}", new StopRoutingHandler()));

            context.MapRoute(
                "ListBuilder_default",
                "ListBuilder/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}