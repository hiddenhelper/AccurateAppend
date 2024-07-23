using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using AccurateAppend.Websites.Admin.Areas.Reporting.Controllers;
 
namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Navigator extensions for the <see cref="DashboardController"/>.
    /// </summary>
    public static class ReportingNavigator
    {
        #region Index

        /// <summary>
        /// Navigates to the <see cref="DashboardController.Index"/> action.
        /// </summary>
        public static ActionResult ToIndex(this ActionNavigator<DashboardController> navigator)
        {
            var action = navigator.RedirectToAction("Index", "Dashboard", new { Area = "Reporting" });
            return action;
        }

        /// <summary>
        /// Navigates to the <see cref="DashboardController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<DashboardController> navigator, String linkText)
        {
            return navigator.ToIndex(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="DashboardController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<DashboardController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "Dashboard", new { Area = "Reporting" }, htmlAttributes);
        }

        #endregion
    }
}