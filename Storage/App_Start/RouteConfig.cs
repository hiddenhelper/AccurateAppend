using System.Web.Mvc;
using System.Web.Routing;

namespace AccurateAppend.Websites.Storage
{
    /// <summary>
    /// MVC route bootstrap routines.
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// Performs registration of all route data.
        /// </summary>
        /// <param name="routes">The application <see cref="RouteCollection"/> that holds the routing tables.</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Upload", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
