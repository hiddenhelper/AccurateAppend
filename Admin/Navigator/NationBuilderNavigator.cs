using System;
using System.Web.Mvc;
using AccurateAppend.Websites.Admin.Areas.NationBuilder.Controllers;
using AccurateAppend.Websites.Admin.Controllers;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Navigator extensions for the <see cref="NationBuilderController"/>.
    /// </summary>
    public static class NationBuilderNavigator
    {
        /// <summary>
        /// Builds a Url to the <see cref="ResumeController.Index"/> action without input parameters.
        /// </summary>
        public static String ToResume(this UrlBuilder<ResumeController> navigator)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Index", "Resume", new {Area = "NationBuilder"});
        }

        /// <summary>
        /// Builds a Url to the <see cref="CancelController.Index"/> action without input parameters.
        /// </summary>
        public static String ToCancel(this UrlBuilder<CancelController> navigator)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Index", "Cancel", new {Area = "NationBuilder"});
        }
    }
}