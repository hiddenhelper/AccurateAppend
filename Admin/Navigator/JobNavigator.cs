using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using AccurateAppend.Websites.Admin.Areas.JobProcessing.ChangeJobPriority;
using AccurateAppend.Websites.Admin.Areas.JobProcessing.DeleteJob;
using AccurateAppend.Websites.Admin.Areas.JobProcessing.Reassign;
using AccurateAppend.Websites.Admin.Areas.JobProcessing.Reset;
using AccurateAppend.Websites.Admin.Areas.JobProcessing.Resume;
using AccurateAppend.Websites.Admin.Areas.JobProcessing.Summary;
using AccurateAppend.Websites.Admin.Controllers;

namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Navigator extensions for the <see cref="SummaryController"/>.
    /// </summary>
    public static class JobsNavigator
    {
        #region JobsController.Index

        /// <summary>
        /// Builds a Url to the <see cref="SummaryController.Index"/> action for the indicated job.
        /// </summary>
        public static String ToIndex(this UrlBuilder<SummaryController> navigator, Int32 jobId)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Index", "Summary", new { area = "JobProcessing", jobId });
        }

        /// <summary>
        /// Builds a Url to the <see cref="SummaryController.Index"/> action for the indicated job.
        /// </summary>
        public static String ToIndex(this UrlBuilder<SummaryController> navigator, Int32 jobId, String scheme)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Index", "Summary", new { area = "JobProcessing", jobId }, scheme);
        }

        /// <summary>
        /// Navigates to the <see cref="SummaryController.Index"/> action.
        /// </summary>
        public static ActionResult ToIndex(this ActionNavigator<SummaryController> navigator)
        {
            var action = navigator.RedirectToAction("Index", "Summary", new {Area = "JobProcessing" });
            return action;
        }

        /// <summary>
        /// Navigates to the <see cref="SummaryController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<SummaryController> navigator, String linkText)
        {
            return navigator.ToIndex(linkText, null);
        }

        /// <summary>
        /// Navigates to the <see cref="SummaryController.Index"/> action.
        /// </summary>
        public static MvcHtmlString ToIndex(this ViewNavigator<SummaryController> navigator, String linkText, Object htmlAttributes)
        {
            return ((IAdapter<HtmlHelper>)navigator).Item.ActionLink(linkText, "Index", "Summary", new { Area = "JobProcessing" }, htmlAttributes);
        }

        #endregion

        #region DeleteJobController.Index

        /// <summary>
        /// Builds a Url to the <see cref="DeleteJobController.Index"/> action without input parameters.
        /// </summary>
        public static String Root(this UrlBuilder<DeleteJobController> navigator)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Index", "DeleteJob", new {area = "JobProcessing"});
        }

        /// <summary>
        /// Builds a Url to the <see cref="DeleteJobController.Index"/> action for the indicated job.
        /// </summary>
        public static String Delete(this UrlBuilder<DeleteJobController> navigator, Int32 jobId)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Index", "DeleteJob", new { area = "JobProcessing", jobId });
        }

        #endregion

        #region ChangeJobPriorityController.Index

        /// <summary>
        /// Builds a Url to the <see cref="ChangeJobPriorityController.Index"/> action without input parameters.
        /// </summary>
        public static String Root(this UrlBuilder<ChangeJobPriorityController> navigator)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Index", "ChangeJobPriority", new {area = "JobProcessing"});
        }

        #endregion

        #region ResetJobController.Index

        /// <summary>
        /// Builds a Url to the <see cref="ResetController.Index"/> action for the indicated job.
        /// </summary>
        public static String Reset(this UrlBuilder<ResetController> navigator, Int32 jobId)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Index", "Reset", new { area = "JobProcessing", jobId });
        }

        #endregion

        #region ResumeJobController.Index

        /// <summary>
        /// Builds a Url to the <see cref="ResumeController.Index"/> action for the indicated job.
        /// </summary>
        public static String Resume(this UrlBuilder<ResumeController> navigator, Int32 jobId)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Index", "Resume", new { area = "JobProcessing", jobId });
        }

        #endregion

        #region ReassignController.Index

        /// <summary>
        /// Builds a Url to the <see cref="ReassignController.Index"/> action for the indicated job.
        /// </summary>
        public static String Reassign(this UrlBuilder<ReassignController> navigator, Int32 jobId)
        {
            var url = ((IAdapter<UrlHelper>)navigator).Item;
            return url.Action("Index", "Reassign", new { area = "JobProcessing", jobId });
        }

        #endregion

        #region ResetJobController.Interactive


        /// <summary>
        /// Navigates to the <see cref="ResetController.Interactive"/> action.
        /// </summary>
        public static ActionResult Interactive(this ActionNavigator<ResetController> navigator, Int32 jobId)
        {
            var action = navigator.RedirectToAction("Interactive", "Reset", new { area = "JobProcessing", jobId });
            return action;
        }

        #endregion
    }
}