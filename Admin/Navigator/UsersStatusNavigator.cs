using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using AccurateAppend.Websites.Admin.Areas.Operations.Controllers;
using AccurateAppend.Websites.Admin.Areas.Operations.UserStatus;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Navigator extensions for the <see cref="UserStatusController"/>.
    /// </summary>
    public static class UserStatusNavigator
    {
        #region Index

        /// <summary>
        /// Build the appropriate URL to the <see cref="UserSummaryController.Index"/> action.
        /// </summary>
        public static String ToIndex(this UrlBuilder<UserStatusController> builder)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Index", "UserStatus", new { Area = "Operations" });
        }

        /// <summary>
        /// Navigates to the <see cref="UserSummaryController.Index"/> action.
        /// </summary>
        public static ActionResult ToIndex(this ActionNavigator<UserStatusController> navigator)
        {
            var action = navigator.RedirectToAction("Index", "UserStatus", new {Area = "Operations" });
            return action;
        }

        /// <summary>
        /// Navigates to the <see cref="UserSummaryController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<UserStatusController> navigator, String linkText)
        {
            return navigator.ToIndex(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="UserSummaryController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<UserStatusController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "UserStatus", new { Area = "Operations" }, htmlAttributes);
        }

        #endregion
    }
}