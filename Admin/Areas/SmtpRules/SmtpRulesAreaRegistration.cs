using System;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Admin.Areas.SmtpRules
{
    /// <summary>
    /// Auto-discovery registration component for the "SmtpRules" area routes.
    /// </summary>
    public class SmtpRulesAreaRegistration : AreaRegistration 
    {
        /// <summary>
        /// Gets the name of the area to register.
        /// </summary>
        /// <returns>
        /// The name of the area to register.
        /// </returns>
        public override String AreaName { get; } = "SmtpRules";

        /// <summary>
        /// Registers an area in an ASP.NET MVC application using the specified area's context information.
        /// </summary>
        /// <param name="context">Encapsulates the information that is required in order to register the area.</param>

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SmtpRules_default",
                "SmtpRules/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}