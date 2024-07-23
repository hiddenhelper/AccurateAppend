using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using AccurateAppend.Websites.Admin.Areas.Tickets.ListTickets;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Navigator extensions for the <see cref="ListTicketsController"/>.
    /// </summary>
    public static class ListTicketsNavigator
    {
        #region ToIndex

        /// <summary>
        /// Build the appropriate URL to the <see cref="ListTicketsController.Index"/> action.
        /// </summary>
        public static String ToIndex(this UrlBuilder<ListTicketsController> builder, String scheme = null)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Index", "ListTickets", new { Area = "Tickets" }, scheme);
        }

        /// <summary>
        /// Navigates to the <see cref="ListTicketsController.Index"/> action.
        /// </summary>
        public static ActionResult ToIndex(this ActionNavigator<ListTicketsController> navigator)
        {
            var action = navigator.RedirectToAction("Index", "ListTickets", new { Area = "Tickets" });
            return action;
        }

        /// <summary>
        /// Navigates to the <see cref="ListTicketsController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<ListTicketsController> navigator, String linkText)
        {
            return navigator.ToIndex(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="ListTicketsController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<ListTicketsController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "ListTickets", new { Area = "Tickets" }, htmlAttributes);
        }

        #endregion
    }
}