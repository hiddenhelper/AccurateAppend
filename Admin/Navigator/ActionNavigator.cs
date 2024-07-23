using System;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Stub object used for extension methods performing <see cref="Controller.RedirectToAction(String, Object)"/> calls with the navigator system from controllers.
    /// </summary>
    /// <typeparam name="T">The <see cref="IController"/> type to perform RedirectToAction calls against.</typeparam>
    public sealed class ActionNavigator<T> : Controller
    {
        /// <summary>
        /// Provides a public proxy to create <see cref="RedirectToRouteResult"/> instances from the contoller system.
        /// </summary>
        /// <param name="actionName">The action to perform.</param>
        /// <param name="controllerName">The controller to perform the action on.</param>
        /// <param name="routeValues">Any route values to use in building the <seealso cref="RedirectToRouteResult"/>.</param>
        /// <returns>The configured <see cref="RedirectToRouteResult"/> instance.</returns>
        public new RedirectToRouteResult RedirectToAction(String actionName, String controllerName, Object routeValues = null)
        {
            return base.RedirectToAction(actionName, controllerName, routeValues);
        }
    }
}