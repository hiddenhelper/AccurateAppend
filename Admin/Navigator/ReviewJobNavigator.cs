using System;
using System.Web.Mvc;
using AccurateAppend.Websites.Admin.Areas.JobProcessing.Review;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Navigator extensions for the <see cref="ReviewController"/>.
    /// </summary>
    public static class ReviewJobNavigator
    {
        /// <summary>
        /// Build the appropriate URL to the <see cref="ReviewController.Index"/> action without input parameters.
        /// </summary>
        public static String ReviewJobRoot(this UrlBuilder<ReviewController> builder)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Index", "Review", new {Area = "JobProcessing"});
        }

        /// <summary>
        /// Build the appropriate URL to the <see cref="ReviewController.Index"/> action.
        /// </summary>
        public static String ReviewJob(this UrlBuilder<ReviewController> builder, Int32 jobId)
        {
            var adapter = builder as IAdapter<UrlHelper>;
            return adapter.Item.Action("Index", "Review", new {Area = "JobProcessing", JobId = jobId});
        }
    }
}