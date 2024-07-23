using System;
using System.IO;
using System.Web;
using System.Web.Routing;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Provides utility methods for crafting bespoke <see cref="HttpContextBase"/> instances on the fly.
    /// </summary>
    /// <remarks>
    /// Generally used in scenarios where a non-ambient context is required by code but the code isn't
    /// actually operating under an HTTP request (such as in a message handler).
    /// </remarks>
    public static class HttpContextUtils
    {
        /// <summary>
        /// Crafts a new <see cref="HttpContextBase"/> instance to call non mutating methods on (e.g. <see cref="HttpServerUtilityBase.MapPath"/>).
        /// </summary>
        public static HttpContextBase CreateShimContext()
        {
            var request = new HttpRequest("/", "http://clients.accurateappend.com", String.Empty);
            var response = new HttpResponse(new StringWriter());
            var httpContext = new HttpContext(request, response);
            var httpContextBase = new HttpContextWrapper(httpContext);

            var routeData = new RouteData();
            var myHostContext = new RequestContext(httpContextBase, routeData);

            return myHostContext.HttpContext;
        }
    }
}