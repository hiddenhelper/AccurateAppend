using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using AccurateAppend.Websites.Admin.Areas.Clients.UserFiles;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Navigator extensions for the <see cref="UserFilesController"/>.
    /// </summary>
    public static class UserFilesNavigator
    {
        /// <summary>
        /// Build the appropriate URL to the <see cref="UserFilesController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<UserFilesController> navigator, String linkText, string email, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "UserFiles", new { Area = "Clients", email }, htmlAttributes);
        }
    }
}