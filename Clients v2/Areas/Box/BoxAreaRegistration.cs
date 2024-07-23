using System;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients.Areas.Box
{
    /// <summary>
    /// Registers the "Box" area.
    /// </summary>
    public class BoxAreaRegistration : AreaRegistration 
    {
        /// <summary>
        /// Gets the name of the area to register.
        /// </summary>
        /// <returns>
        /// The name of the area to register.
        /// </returns>
        public override String AreaName => "Box";

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Box_Enumerate",
                "Box/BoxApi/Enumerate/{regId}",
                new { controller = "BoxApi", action = "Enumerate" }
            );

            context.MapRoute(
                "Box_Details",
                "Box/BoxApi/Details/{regId}",
                new { controller = "BoxApi", action = "Details" }
            );

            context.MapRoute(
                "Box_default",
                "Box/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}