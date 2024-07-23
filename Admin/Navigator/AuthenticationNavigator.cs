using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using AccurateAppend.Websites.Admin.Areas.Operations.Authentication;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Navigator extensions for the <see cref="AuthenticationController"/>.
    /// </summary>
    public static class AuthenticationNavigator
    {
        #region Logon
        
        /// <summary>
        /// Navigates to the <see cref="AuthenticationController.LogOn()"/> action.
        /// </summary>
        public static ActionResult LogOn(this ActionNavigator<AuthenticationController> navigator)
        {
            var action = navigator.RedirectToAction("LogOn", "Authentication", new { Area = "" });
            return action;
        }

        /// <summary>
        /// Navigates to the <see cref="AuthenticationController.LogOn()"/> action.
        /// </summary>
        public static MvcHtmlString LogOn(this ViewNavigator<AuthenticationController> navigator, String linkText)
        {
            return navigator.LogOn(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="AuthenticationController.LogOn()"/> action.
        /// </summary>
        public static MvcHtmlString LogOn(this ViewNavigator<AuthenticationController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "LogOn", "Authentication", new { Area = "" }, htmlAttributes);
        }

        #endregion

        #region LogOut

        /// <summary>
        /// Navigates to the <see cref="AuthenticationController.LogOff()"/> action.
        /// </summary>
        public static ActionResult LogOut(this ActionNavigator<AuthenticationController> navigator)
        {
            var action = navigator.RedirectToAction("LogOff", "Authentication", new { Area = "" });
            return action;
        }

        /// <summary>
        /// Navigates to the <see cref="AuthenticationController.LogOff()"/> action.
        /// </summary>
        public static MvcHtmlString LogOff(this ViewNavigator<AuthenticationController> navigator, String linkText)
        {
            return navigator.LogOff(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="AuthenticationController.LogOff()"/> action.
        /// </summary>
        public static MvcHtmlString LogOff(this ViewNavigator<AuthenticationController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "LogOff", "Authentication", new { Area = "Operations" }, htmlAttributes);
        }

        #endregion
    }
}