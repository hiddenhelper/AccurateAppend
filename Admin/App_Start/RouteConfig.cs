using System.Web.Mvc;
using System.Web.Routing;

namespace AccurateAppend.Websites.Admin
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.xls/{*pathInfo}");
            routes.IgnoreRoute("{*allashx}", new { allashx = @".*\.ashx(/.*)?" });
            routes.IgnoreRoute("Scripts/{*pathInfo}");
            routes.IgnoreRoute("Content/{*pathInfo}");

            routes.IgnoreRoute("{*allExtensionlessAshx}",
                               new
                               {
                                   allExtensionlessAshx =
                                   @"fileultimate/(resource|filemanageraction|fileuploaderformbased)(/.*)?"
                               });

            routes.MapRoute(
                "Job_default", // Route name
                "", // URL with parameters
                new { controller = "Summary", action = "Index" } // Parameter defaults
                ).DataTokens.Add("area", "JobProcessing");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "", action = "", id = UrlParameter.Optional } // Parameter defaults
                );

        }
    }
}