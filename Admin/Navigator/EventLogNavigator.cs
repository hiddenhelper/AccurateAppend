 using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
 using AccurateAppend.Websites.Admin.Areas.Operations.Controllers;
 using AccurateAppend.Websites.Admin.Areas.Operations.EventLog;
 using AccurateAppend.Websites.Admin.Areas.Operations.Message;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Navigator extensions for the <see cref="EventLogController"/>.
    /// </summary>
    public static class EventLogNavigator
    {
        #region Index

        /// <summary>
        /// Navigates to the <see cref="MessageController.Index"/> action.
        /// </summary>
        public static ActionResult ToIndex(this ActionNavigator<EventLogController> navigator)
        {
            var action = navigator.RedirectToAction("Index", "EventLog", new { Area = "Operations" });
            return action;
        }

        /// <summary>
        /// Navigates to the <see cref="EventLogController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<EventLogController> navigator, String linkText)
        {
            return navigator.ToIndex(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="EventLogController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<EventLogController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "EventLog", new { Area = "Operations" }, htmlAttributes);
        }

        #endregion

        #region Legacy
        
        /// <summary>
        /// Navigates to the <see cref="EventLogController.Index"/> action.
        /// </summary>
        public static MvcHtmlString Legacy(this ViewNavigator<EventLogController> navigator, String linkText)
        {
            return navigator.Legacy(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="EventLogController.Index"/> action.
        /// </summary>
        public static MvcHtmlString Legacy(this ViewNavigator<EventLogController> navigator, String linkText, Guid correlationId)
        {
            return navigator.Legacy(linkText, correlationId, null);
        }

        /// <summary>
        /// Navigates to the <see cref="EventLogController.Index"/> action.
        /// </summary>
        public static MvcHtmlString Legacy(this ViewNavigator<EventLogController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "EventLog", new { Area = "Operations" }, htmlAttributes);
        }

        /// <summary>
        /// Navigates to the <see cref="EventLogController.Index"/> action.
        /// </summary>
        public static MvcHtmlString Legacy(this ViewNavigator<EventLogController> navigator, String linkText, Guid correlationId, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "EventLog", new { Area = "Operations", correlationId }, htmlAttributes);
        }

        #endregion
    }
}