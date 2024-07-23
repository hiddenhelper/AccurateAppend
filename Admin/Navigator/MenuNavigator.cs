using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using AccurateAppend.Websites.Admin.Controllers;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Navigator extensions for the <see cref="MenuController"/>.
    /// </summary>
    public static class MenuNavigator
    {
        #region ToUsers

        /// <summary>
        /// Build the appropriate URL to the <see cref="MenuController.ToUsers"/> action.
        /// </summary>
        public static String ToUsers(this UrlBuilder<MenuController> builder)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("ToUsers", "Menu", new { Area = "" });
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToUsers"/> action.
        /// </summary>
        public static MvcHtmlString ToUsers(this ViewNavigator<MenuController> navigator, String linkText)
        {
            return navigator.ToUsers(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToUsers"/> action.
        /// </summary>
        public static MvcHtmlString ToUsers(this ViewNavigator<MenuController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "ToUsers", "Menu", new { Area = "" }, htmlAttributes);
        }

        #endregion

        #region ToLeads

        /// <summary>
        /// Build the appropriate URL to the <see cref="MenuController.ToLeads"/> action.
        /// </summary>
        public static String ToLeads(this UrlBuilder<MenuController> builder)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("ToLeads", "Menu", new { Area = "" });
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToLeads"/> action.
        /// </summary>
        public static MvcHtmlString ToLeads(this ViewNavigator<MenuController> navigator, String linkText)
        {
            return navigator.ToLeads(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToLeads"/> action.
        /// </summary>
        public static MvcHtmlString ToLeads(this ViewNavigator<MenuController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "ToLeads", "Menu", new { Area = "" }, htmlAttributes);
        }

        #endregion

        #region ToDeals

        /// <summary>
        /// Build the appropriate URL to the <see cref="MenuController.ToDeals"/> action.
        /// </summary>
        public static String ToDeals(this UrlBuilder<MenuController> builder)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("ToDeals", "Menu", new { Area = "" });
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToDeals"/> action.
        /// </summary>
        public static MvcHtmlString ToDeals(this ViewNavigator<MenuController> navigator, String linkText)
        {
            return navigator.ToDeals(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToDeals"/> action.
        /// </summary>
        public static MvcHtmlString ToDeals(this ViewNavigator<MenuController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "ToDeals", "Menu", new { Area = "" }, htmlAttributes);
        }

        #endregion

        #region ToOperations

        /// <summary>
        /// Build the appropriate URL to the <see cref="MenuController.ToOperations"/> action.
        /// </summary>
        public static String ToOperations(this UrlBuilder<MenuController> builder)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("ToOperations", "Menu", new { Area = "" });
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToOperations"/> action.
        /// </summary>
        public static MvcHtmlString ToOperations(this ViewNavigator<MenuController> navigator, String linkText)
        {
            return navigator.ToOperations(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToOperations"/> action.
        /// </summary>
        public static MvcHtmlString ToOperations(this ViewNavigator<MenuController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "ToOperations", "Menu", new { Area = "" }, htmlAttributes);
        }

        #endregion

        #region ToFiles

        /// <summary>
        /// Build the appropriate URL to the <see cref="MenuController.ToFiles"/> action.
        /// </summary>
        public static String ToFiles(this UrlBuilder<MenuController> builder)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("ToFiles", "Menu", new { Area = "" });
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToFiles"/> action.
        /// </summary>
        public static MvcHtmlString ToFiles(this ViewNavigator<MenuController> navigator, String linkText)
        {
            return navigator.ToFiles(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToFiles"/> action.
        /// </summary>
        public static MvcHtmlString ToFiles(this ViewNavigator<MenuController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "ToFiles", "Menu", new { Area = "" }, htmlAttributes);
        }

        #endregion

        #region ToJobs

        /// <summary>
        /// Build the appropriate URL to the <see cref="MenuController.ToJobs"/> action.
        /// </summary>
        public static String ToJobs(this UrlBuilder<MenuController> builder)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("ToJobs", "Menu", new { Area = "" });
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToJobs"/> action.
        /// </summary>
        public static MvcHtmlString ToJobs(this ViewNavigator<MenuController> navigator, String linkText)
        {
            return navigator.ToJobs(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToJobs"/> action.
        /// </summary>
        public static MvcHtmlString ToJobs(this ViewNavigator<MenuController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "ToJobs", "Menu", new { Area = "" }, htmlAttributes);
        }

        #endregion

        #region ToMessages

        /// <summary>
        /// Build the appropriate URL to the <see cref="MenuController.ToMessages"/> action.
        /// </summary>
        public static String ToMessages(this UrlBuilder<MenuController> builder)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("ToMessages", "Menu", new { Area = "" });
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToMessages"/> action.
        /// </summary>
        public static MvcHtmlString ToMessages(this ViewNavigator<MenuController> navigator, String linkText)
        {
            return navigator.ToMessages(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToMessages"/> action.
        /// </summary>
        public static MvcHtmlString ToMessages(this ViewNavigator<MenuController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "ToMessages", "Menu", new { Area = "" }, htmlAttributes);
        }

        #endregion

        #region ToEventLog

        /// <summary>
        /// Build the appropriate URL to the <see cref="MenuController.ToEventLog"/> action.
        /// </summary>
        public static String ToEventLog(this UrlBuilder<MenuController> builder)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("ToEventLog", "Menu", new { Area = "" });
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToEventLog"/> action.
        /// </summary>
        public static MvcHtmlString ToEventLog(this ViewNavigator<MenuController> navigator, String linkText)
        {
            return navigator.ToEventLog(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToEventLog"/> action.
        /// </summary>
        public static MvcHtmlString ToEventLog(this ViewNavigator<MenuController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "ToEventLog", "Menu", new { Area = "" }, htmlAttributes);
        }

        #endregion

        #region ToListBuilder

        /// <summary>
        /// Build the appropriate URL to the <see cref="MenuController.ToListBuilder"/> action.
        /// </summary>
        public static String ToListBuilder(this UrlBuilder<MenuController> builder)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("ToListBuilder", "Menu", new { Area = "" });
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToListBuilder"/> action.
        /// </summary>
        public static MvcHtmlString ToListBuilder(this ViewNavigator<MenuController> navigator, String linkText)
        {
            return navigator.ToListBuilder(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToListBuilder"/> action.
        /// </summary>
        public static MvcHtmlString ToListBuilder(this ViewNavigator<MenuController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "ToListBuilder", "Menu", new { Area = "" }, htmlAttributes);
        }

        #endregion

        #region ToChargeEvents

        /// <summary>
        /// Build the appropriate URL to the <see cref="MenuController.ToWebServicesReporting"/> action.
        /// </summary>
        public static String ToChargeEvents(this UrlBuilder<MenuController> builder)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("ToChargeEvents", "Menu", new { Area = "" });
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToWebServicesReporting"/> action.
        /// </summary>
        public static MvcHtmlString ToChargeEvents(this ViewNavigator<MenuController> navigator, String linkText)
        {
            return navigator.ToListBuilder(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToWebServicesReporting"/> action.
        /// </summary>
        public static MvcHtmlString ToChargeEvents(this ViewNavigator<MenuController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "ToChargeEvents", "Menu", new { Area = "" }, htmlAttributes);
        }

        #endregion

        #region ToUserActivity

        /// <summary>
        /// Build the appropriate URL to the <see cref="MenuController.ToUserActivity"/> action.
        /// </summary>
        public static String ToUserActivity(this UrlBuilder<MenuController> builder)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("ToUserActivity", "Menu", new { Area = "" });
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToUserActivity"/> action.
        /// </summary>
        public static MvcHtmlString ToUserActivity(this ViewNavigator<MenuController> navigator, String linkText)
        {
            return navigator.ToEventLog(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToUserActivity"/> action.
        /// </summary>
        public static MvcHtmlString ToUserActivity(this ViewNavigator<MenuController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "ToUserActivity", "Menu", new { Area = "" }, htmlAttributes);
        }

        #endregion

        #region ToReporting

        /// <summary>
        /// Build the appropriate URL to the <see cref="MenuController.ToReporting"/> action.
        /// </summary>
        public static String ToReporting(this UrlBuilder<MenuController> builder)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("ToReporting", "Menu", new { Area = "" });
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToReporting"/> action.
        /// </summary>
        public static MvcHtmlString ToReporting(this ViewNavigator<MenuController> navigator, String linkText)
        {
            return navigator.ToReporting(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToReporting"/> action.
        /// </summary>
        public static MvcHtmlString ToReporting(this ViewNavigator<MenuController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "ToReporting", "Menu", new { Area = "" }, htmlAttributes);
        }

        #endregion

        #region ToTickets

        /// <summary>
        /// Build the appropriate URL to the <see cref="MenuController.ToTickets"/> action.
        /// </summary>
        public static String ToTickets(this UrlBuilder<MenuController> builder)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("ToTickets", "Menu", new { Area = "" });
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToTickets"/> action.
        /// </summary>
        public static MvcHtmlString ToTickets(this ViewNavigator<MenuController> navigator, String linkText)
        {
            return navigator.ToTickets(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToTickets"/> action.
        /// </summary>
        public static MvcHtmlString ToTickets(this ViewNavigator<MenuController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "ToTickets", "Menu", new { Area = "" }, htmlAttributes);
        }

        #endregion

        #region ToSystemConfiguration

        /// <summary>
        /// Build the appropriate URL to the <see cref="MenuController.ToSystemConfiguration"/> action.
        /// </summary>
        public static String ToSystemConfiguration(this UrlBuilder<MenuController> builder)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Index", "Dashboard", new { Area = "Operations" });
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToSystemConfiguration"/> action.
        /// </summary>
        public static MvcHtmlString ToSystemConfiguration(this ViewNavigator<MenuController> navigator, String linkText)
        {
            return navigator.ToSystemConfiguration(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="MenuController.ToSystemConfiguration"/> action.
        /// </summary>
        public static MvcHtmlString ToSystemConfiguration(this ViewNavigator<MenuController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "Dashboard", new { Area = "Operations" }, htmlAttributes);
        }

        #endregion

        
    }
}