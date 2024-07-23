using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using AccurateAppend.Websites.Admin.Areas.Operations.Message;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Navigator extensions for the <see cref="MessageController"/>.
    /// </summary>
    public static class MessageNavigator
    {
        #region Index
        
        /// <summary>
        /// Navigates to the <see cref="MessageController.Index"/> action.
        /// </summary>
        public static ActionResult ToIndex(this ActionNavigator<MessageController> navigator)
        {
            var action = navigator.RedirectToAction("Index", "Message", new { Area = "Operations" });
            return action;
        }

        /// <summary>
        /// Navigates to the <see cref="MessageController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<MessageController> navigator, String linkText)
        {
            return navigator.ToIndex(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="MessageController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<MessageController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "Message", new { Area = "Operations" }, htmlAttributes);
        }

        /// <summary>
        /// Builds a Url to the <see cref="MessageController.Index"/> action without input parameters.
        /// </summary>
        public static String ToIndex(this UrlBuilder<MessageController> navigator)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Index", "Message", new { Area = "Operations" });
        }

        #endregion
       
        #region toJson

        /// <summary>
        /// Builds a Url to the <see cref="MessageController.GetMessagesJson"/> action without input parameters.
        /// </summary>
        public static String GetMessagesJson(this UrlBuilder<MessageController> navigator)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("GetMessagesJson", "Message", new { Area = "Operations" });
        }

        /// <summary>
        /// Builds a Url to the <see cref="MessageController.GetMessageDetailJson"/> action without input parameters.
        /// </summary>
        public static String GetMessageDetailJson(this UrlBuilder<MessageController> navigator)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("GetMessageDetailJson", "Message", new { Area = "Operations" });
        }

        /// <summary>
        /// Builds a Url to the <see cref="MessageController.GetRecentMessageRecipientsJson"/> action without input parameters.
        /// </summary>
        public static String GetRecentMessageRecipientsJson(this UrlBuilder<MessageController> navigator)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("GetRecentMessageRecipientsJson", "Message", new { Area = "Operations" });
        }

        #endregion
    }
}