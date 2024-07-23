using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AccurateAppend.Websites.Admin.Controllers;
using AccurateAppend.Websites.Admin.Areas.JobProcessing.Summary.Models;

namespace AccurateAppend.Websites.Admin.Areas.JobProcessing.Summary
{
    [Authorize()]
    public class SummaryController : ActivityLoggingController2
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