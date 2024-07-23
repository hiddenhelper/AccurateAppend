using System;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients.Areas.Public
{
    /// <summary>
    /// Registers the "Public" area.
    /// </summary>
    public class PublicAreaRegistration : AreaRegistration 
    {
        /// <summary>
        /// Gets the name of the area to register.
        /// </summary>
        /// <returns>
        /// The name of the area to register.
        /// </returns>
        public override String AreaName => "Public";

        /// <summary>Registers an area in an ASP.NET MVC application using the specified area's context information.</summary>
        /// <param name="context">Encapsulates the information that is required in order to register the area.</param>
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Public_payment",
                "Public/Payment/{action}/{userid}",
                new { controller = "Payment", action = "Index", userid = UrlParameter.Optional }
            );

            context.MapRoute(
                "Public_default",
                "Public/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}