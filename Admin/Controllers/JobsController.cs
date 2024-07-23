using System;
using System.Web.Mvc;
using AccurateAppend.Websites.Admin.ViewModels.Job;

namespace AccurateAppend.Websites.Admin.Controllers
{
    [Authorize()]
    public class JobsController : ActivityLoggingController2
    {
        #region Actions

        /// <summary>
        /// View Jobs for a Site
        /// </summary>
        /// <returns></returns>
        [OutputCache(Duration = 0, VaryByParam = "None")]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index(String email, Int32? jobId)
        {
            this.OnEvent("Viewed job queue");

            var model = new JobsRequest() { Email = email, JobId = jobId };
            return this.View(model);
        }

        #endregion
    }
} 