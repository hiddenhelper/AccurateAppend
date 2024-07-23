using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using AccurateAppend.Websites.Admin.Areas.ListBuilder.Controllers;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Navigator extensions for the <see cref="BuildListController"/>.
    /// </summary>
    public static class ListBuilderNavigator
    {
        /// <summary>
        /// Navigates to the <see cref="CriteriaBuilderController.Start"/> action.
        /// </summary>
        public static ActionResult ToIndex(this ActionNavigator<BuildListController> navigator)
        {
            var action = navigator.RedirectToAction("Start", "CriteriaBuilder", new { Area = "ListBuilder" });
            return action;
        }


        /// <summary>
        /// Navigates to the <see cref="CriteriaBuilderController.Start"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<BuildListController> navigator, String linkText)
        {
            return navigator.ToIndex(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="CriteriaBuilderController.Start"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<BuildListController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Start", "CriteriaBuilder", new { Area = "ListBuilder" }, htmlAttributes);
        }
    }
}