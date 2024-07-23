using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using AccurateAppend.Websites.Admin.Areas.SecurityManagement.LogInAsClient;
using AccurateAppend.Websites.Admin.Areas.Clients.SearchClients;
using AccurateAppend.Websites.Admin.Areas.Clients.UserDetail;
using AccurateAppend.Websites.Admin.Areas.Clients.UserSummary;
using AccurateAppend.Websites.Admin.Areas.Operations.UserStatus;
using NHibernate.Engine.Query;
using NHibernate.Mapping;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Navigator extensions for the User data.
    /// </summary>
    public static class UsersNavigator
    {
        #region Index

        /// <summary>
        /// Build the appropriate URL to the <see cref="Index"/> action.
        /// </summary>
        public static String ToIndex(this UrlBuilder<UserSummaryController> builder)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Index", "UserSummary", new { Area = "Clients" });
        }

        /// <summary>
        /// Navigates to the <see cref="UserSummaryController.Index"/> action.
        /// </summary>
        public static ActionResult ToIndex(this ActionNavigator<UserSummaryController> navigator)
        {
            var action = navigator.RedirectToAction("Index", "UserSummary", new {Area = "Clients"});
            return action;
        }

        /// <summary>
        /// Navigates to the <see cref="UserSummaryController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<UserSummaryController> navigator, String linkText)
        {
            return navigator.ToIndex(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="UserSummaryController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<UserSummaryController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "UserSummary", new { Area = "Clients" }, htmlAttributes);
        }

        #endregion

        #region Detail

        /// <summary>
        /// Build the appropriate URL to the <see cref="CallableParser.Detail"/> action.
        /// </summary>
        public static String ToDetail(this UrlBuilder<UserDetailController> builder, Guid userId, String scheme = null)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Index", "UserDetail", new { Area = "Clients", UserId = userId }, scheme);
        }

        /// <summary>
        /// Navigates to the <see cref="CallableParser.Detail"/> action.
        /// </summary>
        public static ActionResult Detail(this ActionNavigator<UserDetailController> navigator, Guid userId)
        {
            var action = navigator.RedirectToAction("Index", "UserDetail", new { Area = "Clients", UserId = userId });
            return action;
        }

        /// <summary>
        /// Navigates to the <see cref="CallableParser.Detail"/> action.
        /// </summary>
        public static MvcHtmlString Detail(this ViewNavigator<UserDetailController> navigator, String linkText, Guid userId)
        {
            return navigator.Detail(linkText, userId, null);
        }

        /// <summary>
        /// Navigates to the <see cref="CallableParser.Detail"/> action.
        /// </summary>
        public static MvcHtmlString Detail(this ViewNavigator<UserDetailController> navigator, String linkText, Guid userId, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "UserDetail", new { Area = "Clients", userId }, htmlAttributes);
        }

        /// <summary>
        /// Navigates to the <see cref="CallableParser.Detail"/> action.
        /// </summary>
        public static MvcHtmlString Detail(this ViewNavigator<UserDetailController> navigator, String linkText, Int32 leadId)
        {
            return navigator.Detail(linkText, leadId, null);
        }

        /// <summary>
        /// Navigates to the <see cref="CallableParser.Detail"/> action.
        /// </summary>
        public static MvcHtmlString Detail(this ViewNavigator<UserDetailController> navigator, String linkText, Int32 leadId, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "FromLead", "UserDetail", new { Area = "Clients", LeadId = leadId }, htmlAttributes);
        }

        #endregion

        #region User Status

        /// <summary>
        /// Build the appropriate URL to the <see cref="UserStatusController.Index"/> action.
        /// </summary>
        public static String Status(this UrlBuilder<UserStatusController> builder)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Index", "UserStatus", new { Area = "Operations" });
        }

        /// <summary>
        /// Build the appropriate URL to the <see cref="Index"/> action.
        /// </summary>
        public static String Search(this UrlBuilder<SearchClientsController> builder)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Index", "SearchClients", new { Area = "Clients" });
        }

        #endregion

        #region LoginAsClient

        /// <summary>
        /// Build the appropriate URL to the <see cref="Index"/> action.
        /// </summary>
        public static String LoginAsClient(this UrlBuilder<LogInAsClientController> builder, Guid userId)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Index", "LogInAsClient", new { Area = "SecurityManagement", userId });
        }

        /// <summary>
        /// Navigates to the <see cref="LogInAsClientController.Index"/> action.
        /// </summary>
        public static ActionResult LoginAsClient(this ActionNavigator<LogInAsClientController> navigator, Guid userId)
        {
            var action = navigator.RedirectToAction("Index", "LogInAsClient", new { Area = "SecurityManagement", userId });
            return action;
        }

        /// <summary>
        /// Navigates to the <see cref="LogInAsClientController.Index"/> action.
        /// </summary>
        public static MvcHtmlString LoginAsClient(this ViewNavigator<LogInAsClientController> navigator, Guid userId, String linkText)
        {
            return navigator.LoginAsClient(userId, linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="LogInAsClientController.Index"/> action.
        /// </summary>
        public static MvcHtmlString LoginAsClient(this ViewNavigator<LogInAsClientController> navigator, Guid userId, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "LogInAsClient", new { Area = "SecurityManagement", userId }, htmlAttributes);
        }

        #endregion
    }
}