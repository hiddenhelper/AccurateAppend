using System;
using System.Reflection;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Containers standard routines for error display for controllers.
    /// </summary>
    public static class ControllerExtensions
    {
        private static readonly MethodInfo Method = typeof(Controller).GetMethod("View", BindingFlags.Instance | BindingFlags.NonPublic, null, new[] {typeof(String)}, null);

        /// <summary>
        /// Creates an <see cref="ActionResult"/> for use in displaying an error message to the end user. Usually
        /// occurs when an unhandled exception.
        /// </summary>
        public static ActionResult DisplayErrorResult(this Controller controller, String userMessage = null)
        {
            controller.TempData["message"] = userMessage;

            return (ActionResult)Method.Invoke(controller, new Object[] { "~/Views/Shared/Error.aspx" });
        }
    }
}