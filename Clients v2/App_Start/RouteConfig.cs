using System.Web.Mvc;
using System.Web.Routing;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// MVC route bootstrap routines.
    /// </summary>
    public static class RouteConfig
    {
        /// <summary>
        /// Performs registration of all route data.
        /// </summary>
        /// <param name="routes">The application <see cref="RouteCollection"/> that holds the routing tables.</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("favicon.ico");
            routes.IgnoreRoute("robots.txt");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("Scripts/{*pathInfo}");
            routes.IgnoreRoute("Content/{*pathInfo}");

            var route = routes.MapRoute(
                name: "Default",
                url: "",
                defaults: new {controller = "Default", action = "Index", id = UrlParameter.Optional}
            );

            route.DataTokens["area"] = "Public";

            routes.MapRoute(
                name: "Default_root",
                url: "{controller}/{action}/{id}",
                defaults: new {action = "Index", id = UrlParameter.Optional}
            );
        }
    }
}