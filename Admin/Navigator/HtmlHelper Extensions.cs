using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Extensions for the <see cref="HtmlHelper"/> class that are only used in the navigator system.
    /// </summary>
    internal static class HtmlHelperExtensions
    {
        #region ActionUrl

        public static String ActionUrl(this HtmlHelper html, String actionName, String controllerName, Object routeValues)
        {
            return ActionUrl(html, actionName, controllerName, new RouteValueDictionary(routeValues));
        }

        public static String ActionUrl(this HtmlHelper html, String actionName, String controllerName, RouteValueDictionary routeValues)
        {
            var url = UrlHelper.GenerateUrl(null, actionName, controllerName, null, null, null, routeValues, html.RouteCollection, html.ViewContext.RequestContext, true);
            return url;
        }

        #endregion
    }
}