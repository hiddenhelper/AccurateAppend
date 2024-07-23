using System;
using System.Web.Mvc;
using AccurateAppend.Websites.Admin.Areas.Clients.AdminFile;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Navigator extensions for the <see cref="AdminFileController"/>.
    /// </summary>
    public static class AdminFileNavigator
    {
        /// <summary>
        /// Builds a Url to the <see cref="AdminFileController.Summary"/> action without input parameters.
        /// </summary>
        public static String Summary(this UrlBuilder<AdminFileController> navigator, Guid userid)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Summary", "AdminFile", new { Area = "Clients", UserId = userid });
        }

        /// <summary>
        /// Builds a Url to the <see cref="AdminFileController.Summary"/> action without input parameters.
        /// </summary>
        public static String Summary(this UrlBuilder<AdminFileController> navigator)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Summary", "AdminFile", new { Area = "Clients" });
        }

        /// <summary>
        /// Builds a Url to the <see cref="AdminFileController.Save"/> action without input parameters.
        /// </summary>
        public static String Save(this UrlBuilder<AdminFileController> navigator, Guid userid)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Save", "AdminFile", new { Area = "Clients", UserId = userid });
        }

        /// <summary>
        /// Builds a Url to the <see cref="AdminFileController.Download"/> action without input parameters.
        /// </summary>
        public static String Download(this UrlBuilder<AdminFileController> navigator, int fileid)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Download", "AdminFile", new { Area = "Clients", FileId = fileid });
        }

        /// <summary>
        /// Builds a Url to the <see cref="AdminFileController.Download"/> action without input parameters.
        /// </summary>
        public static String Download(this UrlBuilder<AdminFileController> navigator)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Download", "AdminFile", new { Area = "Clients" });
        }

        /// <summary>
        /// Builds a Url to the <see cref="AdminFileController.Delete"/> action without input parameters.
        /// </summary>
        public static String Delete(this UrlBuilder<AdminFileController> navigator, int fileid)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Delete", "AdminFile", new { Area = "Clients", FileId = fileid });
        }

        /// <summary>
        /// Builds a Url to the <see cref="AdminFileController.Delete"/> action without input parameters.
        /// </summary>
        public static String Delete(this UrlBuilder<AdminFileController> navigator)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Delete", "AdminFile", new { Area = "Clients" });
        }

        /// <summary>
        /// Builds a Url to the <see cref="AdminFileController.AddNote"/> action without input parameters.
        /// </summary>
        public static String AddNote(this UrlBuilder<AdminFileController> navigator, int fileid, string notes)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("AddNote", "AdminFile", new { Area = "Clients", FileId = fileid, Notes = notes });
        }

        /// <summary>
        /// Builds a Url to the <see cref="AdminFileController.AddNote"/> action without input parameters.
        /// </summary>
        public static String AddNote(this UrlBuilder<AdminFileController> navigator)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("AddNote", "AdminFile", new { Area = "Clients" });
        }
    }
}