using System;
using System.Diagnostics.Contracts;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using AccurateAppend.Websites.Admin.Areas.Clients.DeleteUserFile;
using AccurateAppend.Websites.Admin.Areas.Clients.DownloadUserFile;
using AccurateAppend.Websites.Admin.Areas.Clients.UserFileDetail;
using AccurateAppend.Websites.Admin.Areas.Clients.UserFiles;
using NHibernate.Mapping;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Navigator extensions for the User Files views.
    /// </summary>
    public static class FilesNavigator
    {
        #region Index

        /// <summary>
        /// Navigates to the <see cref="Index"/> action.
        /// </summary>
        public static ActionResult ToIndex(this ActionNavigator<UserFilesController> navigator)
        {
            var action = navigator.RedirectToAction("Index", "UserFiles", new {Area = "Clients"});
            return action;
        }

        /// <summary>
        /// Navigates to the <see cref="UserFilesController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<UserFilesController> navigator, String linkText)
        {
            return navigator.ToIndex(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="UserFilesController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<UserFilesController> navigator, String linkText, String email)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "UserFiles", new { Area = "Clients", email }, null);
        }

        /// <summary>
        /// Navigates to the <see cref="UserFilesController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<UserFilesController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "UserFiles", new { Area = "Clients" }, htmlAttributes);
        }

        /// <summary>
        /// Builds a Url to the <see cref="UserFilesController.Index"/> action without input parameters.
        /// </summary>
        public static String ToIndex(this UrlBuilder<UserFilesController> navigator)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Index", "UserFiles", new { Area = "Clients" });
        }

        #endregion

        #region DeleteFile

        /// <summary>
        /// Build the appropriate URL to the <see cref="Index"/> action.
        /// </summary>
        public static String Delete(this UrlBuilder<DeleteUserFileController> navigator)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Index", "DeleteUserFile", new { Area = "Clients" });
        }

        #endregion

        #region Download

        /// <summary>
        /// Creates an action link to the <see cref="Index"/> action.
        /// </summary>
        public static String Download(this UrlBuilder<DownloadUserFileController> builder, Guid publicKey)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            Contract.EndContractBlock();

            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Index", "DownloadUserFile", new {Area="Clients", publicKey});
        }

        /// <summary>
        /// Creates an action link to the <see cref="DownloadUserFileController.Index"/> action.
        /// </summary>
        public static String Download(this UrlBuilder<DownloadUserFileController> builder, String systemFileName)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            Contract.EndContractBlock();

            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Index", "DownloadUserFile", new { Area = "Clients", systemFileName });
        }

        #endregion

        #region GetJson

        /// <summary>
        /// Builds a Url to the <see cref="UserFileDetailController.Index"/> action for the indicated file.
        /// </summary>
        public static String GetDetail(this UrlBuilder<UserFileDetailController> navigator, Guid publicKey)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Index", "UserFileDetail", new { Area = "Clients", id = publicKey });
        }

        #endregion
    }
}