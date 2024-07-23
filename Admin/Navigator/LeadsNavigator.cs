using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using AccurateAppend.Websites.Admin.Areas.Clients.DeleteLead;
using AccurateAppend.Websites.Admin.Areas.Clients.LeadDetail;
using AccurateAppend.Websites.Admin.Areas.Clients.LeadSummary;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Navigator extensions for the <see cref="LeadDetailController"/>, <see cref="LeadSummaryController"/>, and <see cref="DeleteLeadController"/>.
    /// </summary>
    public static class LeadsNavigator
    {
        #region Delete

        /// <summary>
        /// Build the appropriate URL to the <see cref="DeleteLeadController.Index"/> action.
        /// </summary>
        public static String Delete(this UrlBuilder<DeleteLeadController> builder, Int32 leadId, String scheme = null)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Index", "DeleteLead", new { Area = "Clients", leadId }, scheme);
        }

        /// <summary>
        /// Navigates to the <see cref="DeleteLeadController.Index"/> action.
        /// </summary>
        public static ActionResult Delete(this ActionNavigator<DeleteLeadController> navigator, Int32 leadId)
        {
            var action = navigator.RedirectToAction("Index", "DeleteLead", new { Area = "Clients", leadId });
            return action;
        }

        /// <summary>
        /// Navigates to the <see cref="DeleteLeadController.Index"/> action.
        /// </summary>
        public static MvcHtmlString Delete(this ViewNavigator<DeleteLeadController> navigator, Int32 leadId, String linkText)
        {
            return navigator.Delete(leadId, linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="DeleteLeadController.Index"/> action.
        /// </summary>
        public static MvcHtmlString Delete(this ViewNavigator<DeleteLeadController> navigator, Int32 leadId, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "DeleteLead", new { Area = "Clients", leadId }, htmlAttributes);
        }

        #endregion

        #region ToDetail

        /// <summary>
        /// Build the appropriate URL to the <see cref="LeadDetailController.View"/> action.
        /// </summary>
        public static String ToDetail(this UrlBuilder<LeadDetailController> builder, Int32 leadId, String scheme = null)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("View", "LeadDetail", new { Area = "Clients", leadId }, scheme);
        }

        /// <summary>
        /// Navigates to the <see cref="LeadDetailController.View"/> action.
        /// </summary>
        public static ActionResult ToDetail(this ActionNavigator<LeadDetailController> navigator, Int32 leadId)
        {
            var action = navigator.RedirectToAction("View", "LeadDetail", new { Area = "Clients", leadId });
            return action;
        }

        /// <summary>
        /// Navigates to the <see cref="LeadDetailController.View"/> action.
        /// </summary>
        public static MvcHtmlString ToDetail(this ViewNavigator<LeadDetailController> navigator, Int32 leadId, String linkText)
        {
            return navigator.ToDetail(leadId, linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="LeadDetailController.View"/> action.
        /// </summary>
        public static MvcHtmlString ToDetail(this ViewNavigator<LeadDetailController> navigator, Int32 leadId, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "View", "LeadDetail", new { Area = "Clients", leadId }, htmlAttributes);
        }

        #endregion

        #region ToIndex

        /// <summary>
        /// Build the appropriate URL to the <see cref="LeadSummaryController.Index"/> action.
        /// </summary>
        public static String ToIndex(this UrlBuilder<LeadSummaryController> builder, String scheme = null)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Index", "LeadSummary", new { Area = "Clients" }, scheme);
        }

        /// <summary>
        /// Navigates to the <see cref="LeadSummaryController.Index"/> action.
        /// </summary>
        public static ActionResult ToIndex(this ActionNavigator<LeadSummaryController> navigator)
        {
            var action = navigator.RedirectToAction("Index", "LeadSummary", new {Area = "Clients"});
            return action;
        }

        /// <summary>
        /// Navigates to the <see cref="LeadSummaryController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<LeadSummaryController> navigator, String linkText)
        {
            return navigator.ToIndex(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="LeadSummaryController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<LeadSummaryController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "LeadSummary", new { Area = "Clients" }, htmlAttributes);
        }

        #endregion

        #region Create

        /// <summary>
        /// Build the appropriate URL to the <see cref="LeadDetailController.Create"/> action.
        /// </summary>
        public static String Create(this UrlBuilder<LeadDetailController> builder, String scheme = null)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Create", "LeadDetail", new { Area = "Clients" }, scheme);
        }

        /// <summary>
        /// Navigates to the <see cref="LeadDetailController.Create"/> action.
        /// </summary>
        public static ActionResult Create(this ActionNavigator<LeadDetailController> navigator)
        {
            var action = navigator.RedirectToAction("Create", "LeadDetail", new { Area = "Clients" });
            return action;
        }

        /// <summary>
        /// Navigates to the <see cref="LeadDetailController.Create"/> action.
        /// </summary>
        public static MvcHtmlString Create(this ViewNavigator<LeadDetailController> navigator, String linkText)
        {
            return navigator.Create(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="LeadDetailController.Create"/> action.
        /// </summary>
        public static MvcHtmlString Create(this ViewNavigator<LeadDetailController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Create", "LeadDetail", new { Area = "Clients" }, htmlAttributes);
        }

        #endregion
    }
}