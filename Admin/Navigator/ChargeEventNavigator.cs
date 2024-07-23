using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using AccurateAppend.Websites.Admin.Areas.Sales.ChargeEventSummary;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Navigator extensions for the various charge event related controllers, 
    /// <see cref="ChargeEventSummaryController"/>).
    /// </summary>
    public static class ChargeEventNavigator
    {
        /// <summary>
        /// Navigates to the <see cref="ChargeEventSummaryController.Index"/> action.
        /// </summary>
        public static String ForDeal(this UrlBuilder<ChargeEventSummaryController> builder, Int32 dealId)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Index", "ChargeEventSummary", new { Area = "Sales", dealId });
        }

        #region ChargeEventSummary.Index

        /// <summary>
        /// Navigates to the <see cref="ChargeEventSummaryController.Index"/> action.
        /// </summary>
        public static ActionResult ToIndex(this ActionNavigator<ChargeEventSummaryController> navigator)
        {
            var action = navigator.RedirectToAction("Index", "ChargeEventSummary", new { Area = "Sales" });
            return action;
        }

        /// <summary>
        /// Build the appropriate URL to the <see cref="ChargeEventSummaryController.Index"/> action.
        /// </summary>
        public static String ToIndex(this UrlBuilder<ChargeEventSummaryController> builder)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Index", "ChargeEventSummary", new { Area = "Sales" });
        }

        /// <summary>
        /// Navigates to the <see cref="ChargeEventSummaryController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<ChargeEventSummaryController> navigator, String linkText, Guid userId)
        {
            return navigator.ToIndex(linkText, userId, null);
        }

        /// <summary>
        /// Navigates to the <see cref="ChargeEventSummaryController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<ChargeEventSummaryController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "ChargeEventSummary", new { Area = "Sales" }, htmlAttributes);
        }

        /// <summary>
        /// Navigates to the <see cref="ChargeEventSummaryController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<ChargeEventSummaryController> navigator, String linkText, Guid userId, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "ChargeEventSummary", new { Area = "Sales", UserId = userId }, htmlAttributes);
        }

        /// <summary>
        /// Navigates to the <see cref="ChargeEventSummaryController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<ChargeEventSummaryController> navigator, String linkText, String email, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "ChargeEventSummary", new { Area = "Sales", email }, htmlAttributes);
        }

        #endregion
        
        #region ChargeEventSummary.Json
        
        /// <summary>
        /// Builds a Url to the <see cref="ChargeEventSummaryController.Query"/> action without input parameters.
        /// </summary>
        public static String GetChargeEventsJson(this UrlBuilder<ChargeEventSummaryController> navigator)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Query", "ChargeEventSummary", new { Area = "Sales" });
        }

        /// <summary>
        /// Builds a Url to the <see cref="ChargeEventSummaryController.GetChargeEventsStatusesJson"/> action without input parameters.
        /// </summary>
        public static String GetChargeEventsStatusesJson(this UrlBuilder<ChargeEventSummaryController> navigator)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("GetChargeEventsStatusesJson", "ChargeEventSummary", new { Area = "Sales" });
        }

        #endregion

    }
}