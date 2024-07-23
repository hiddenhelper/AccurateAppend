using System;
using System.Linq;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients
{
    /// <summary>
    /// Provides the ability to examine the current request url and prevent access to the action.
    /// </summary>
    /// <remarks>
    /// Due to the limitations of the <see cref="ActionFilterAttribute"/> and the inability to leverage arrays
    /// for settings this type is used to define the restricted hosts. Use the <see cref="RestrictedAttribute"/>
    /// to actually restrict access based on these settings.
    /// </remarks>
    [Serializable()]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class RestrictUrlAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestrictUrlAttribute"/> class.  
        /// </summary>
        /// <param name="host">The host name portion the request url is be restricted from.</param>
        /// <param name="redirectTo">The url that the client should be redirected to.</param>
        public RestrictUrlAttribute(String host, String redirectTo)
        {
            if (String.IsNullOrWhiteSpace(host)) throw new ArgumentNullException(nameof(host));
            if (String.IsNullOrWhiteSpace(redirectTo)) throw new ArgumentNullException(nameof(redirectTo));

            this.Host = host.Trim();
            this.RedirectTo = redirectTo.Trim();
        }
        
        /// <summary>
        /// Gets the host name portion the request url is be restricted from. 
        /// </summary>
        public String Host { get; }

        /// <summary>
        /// Gets the url the client should be redirected to if not allowed.
        /// </summary>
        public String RedirectTo { get; }

        /// <inheritdoc />
        public override String ToString()
        {
            return $"Restricting access to host: {this.Host}";
        }

        /// <summary>
        /// Evaluates the supplied <paramref name="url"/> to determine if the host is restricted from this controller.
        /// </summary>
        /// <param name="url">The current url of the request.</param>
        /// <returns>True if the request is allowed; Otherwise false.</returns>
        public Boolean IsAllowed(Uri url)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));

            var hostName = url.Host;

            if (this.Host.Equals(hostName, StringComparison.OrdinalIgnoreCase)) return false;

            return true;
        }
    }

    /// <summary>
    /// Provides the ability to examine the current request url and prevent access to the action.
    /// </summary>
    /// <remarks>
    /// Examines the executing controller for the presence of any <see cref="RestrictUrlAttribute"/>
    /// items. The first instance that restricts access will be stop execution of the current request
    /// and produce a <see cref="RedirectResult"/> to the indicated url.
    /// </remarks>
    [Serializable()]
    public sealed class RestrictedAttribute : ActionFilterAttribute
    {
        /// <summary>Called by the ASP.NET MVC framework before the action method executes.</summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.Controller;

            var attributes = GetCustomAttributes(controller.GetType()).OfType<RestrictUrlAttribute>().ToArray();
            if (attributes.Any())
            {
                var url = filterContext.HttpContext.Request.Url ?? new Uri("https:\\localhost");

                var isAllowed = attributes.FirstOrDefault(a => !a.IsAllowed(url));
                if (isAllowed != null)
                {
                    filterContext.Result = new RedirectResult(isAllowed.RedirectTo);
                    return;
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}