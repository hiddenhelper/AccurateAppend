using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core;
using AccurateAppend.Data;
using AccurateAppend.Websites.Admin.Areas.Clients.UserDetail;
using AccurateAppend.Websites.Admin.Controllers;
using AccurateAppend.Websites.Admin.Navigator;
using DomainModel.ActionResults;
using DomainModel.Queries;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace AccurateAppend.Websites.Admin.Areas.JobProcessing.Queue
{
    /// <summary>
    /// Controller for querying the job processing queue.
    /// </summary>
    [Authorize()]
    public class QueueController : Controller
    {
        private readonly IJobQueueQuery dal;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueueController"/> class.
        /// </summary>
        /// <param name="dal">The <see cref="IJobQueueQuery"/> providing data access.</param>
        public QueueController(IJobQueueQuery dal)
        {
            if (dal == null) throw new ArgumentNullException(nameof(dal));
            Contract.EndContractBlock();

            this.dal = dal;
        }

        /// <summary>
        /// Returns InProcess jobs for site as json.
        /// </summary>
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> InProcess(
            [DataSourceRequest] DataSourceRequest request,
            Guid applicationid,
            Guid? userid,
            Int32? jobid,
            CancellationToken cancellation)
        {
            var query = this.dal.InProgress(applicationid);

            if (userid.HasValue)
            {
                query = query.Where(j => j.UserId == userid.Value);
            }

            if (jobid.HasValue)
            {
                query = query.Where(j => j.JobId == jobid.Value);
            }

            var final = (await query
                    .OrderBy(a => a.Priority)
                    .ThenBy(a => a.DateSubmitted)
                    .ToArrayAsync(cancellation))
                .Select(j => new
                {
                    j.JobId,
                    j.UserId,
                    DateSubmitted = j.DateSubmitted.ToUserLocal(),
                    DateUpdated = j.DateUpdated.ToUserLocal(),
                    DateComplete = j.DateComplete.ToUserLocal(),
                    j.RecordCount,
                    j.ProcessedCount,
                    j.MatchCount,
                    j.Status,
                    j.CustomerFileName,
                    j.InputFileName,
                    j.Source,
                    j.UserName,
                    j.MatchRate,
                    SourceDescription = j.Source.GetDescription(),
                    SubmittedDescription = j.DateSubmitted.ToSafeLocal().DescribeDifference(DateTime.Now),
                    Progress = new {j.EstimatedCompletionTime},
                    StatusDescription = j.Status.GetDescription() + (j.IsPaused ? " (paused)" : String.Empty),
                    Links = new
                    {
                        UserDetail = this.Url.BuildFor<UserDetailController>().ToDetail(j.UserId),
                        JobsForClient = this.Url.Action("Index", "Summary", new { Area = "JobProcessing", email = j.UserName }),
                        JobDetail = this.Url.Action("Index", "Detail", new { Area = "JobProcessing", jobId = j.JobId }),
                        SetJobComplete = this.Url.Action("Index", "Review", new { Area = "JobProcessing", jobId = j.JobId }),
                        ChangeJobPriority = this.Url.Action("Index", "ChangeJobPriority", new { Area = "JobProcessing", jobId = j.JobId }),
                    }
                })
                .ToArray();

            var data = final.ToDataSourceResult(request);
            data.Total = final.Length;

            var jsonNetResult = new JsonNetResult(DateTimeKind.Local)
            {
                Data = data
            };
            return jsonNetResult;
        }

        /// <summary>
        /// Returns Processed jobs for site as json
        /// </summary>
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public virtual async Task<ActionResult> CompleteSummary(
            [DataSourceRequest] DataSourceRequest request,
            Guid applicationid,
            Guid? userid,
            DateTime startdate,
            DateTime enddate,
            CancellationToken cancellation)
        {
            startdate = startdate.ToStartOfDay();
            enddate = enddate.ToEndOfDay();

            var query = this.dal.CompletedSummary(applicationid, startdate, enddate);

            if (userid != null)
            {
                query = query.Where(j => j.UserId == userid.Value);
            }

            var final = (await query.OrderByDescending(j => j.LastActivity)
                .ToArrayAsync(cancellation))
                .Select(j => new
                {
                    j.FileCount,
                    j.LastActivity,
                    LastActivityDescription = j.LastActivity.DescribeDifference(DateTime.Now),
                    j.MatchCount,
                    j.RecordCount,
                    j.UserId,
                    j.UserName,
                    Links = new
                    {
                        UserDetail = this.Url.BuildFor<UserDetailController>().ToDetail(j.UserId),
                        JobsForClient = this.Url.Action("Index", "Summary", new { Area = "JobProcessing", email = j.UserName }),
                        NewJob = this.Url.Action("DynamicAppend", "Batch", new { Area = "" })
                    }
                }).ToArray();

            var data = final.ToDataSourceResult(request);
            data.Total = final.Length;

            var jsonNetResult = new JsonNetResult(DateTimeKind.Local)
            {
                Data = data
            };
            return jsonNetResult;
        }

        /// <summary>
        /// Gets Jobs for a specific user
        /// </summary>
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public virtual async Task<ActionResult> Complete(
            [DataSourceRequest] DataSourceRequest request,
            Guid? userid,
            DateTime startdate,
            DateTime enddate,
            String email,
            CancellationToken cancellation)
        {
            startdate = startdate.ToStartOfDay();
            enddate = enddate.ToEndOfDay();

            IQueryable<DomainModel.ReadModel.JobQueueView> query;

            if (!String.IsNullOrEmpty(email))
            {
                query = this.dal.CompletedDuring(email, startdate, enddate);
            }
            else if (userid.HasValue)
            {
                query = this.dal.CompletedDuring(userid.Value, startdate, enddate);
            }
            else
            {
                query = Enumerable.Empty<DomainModel.ReadModel.JobQueueView>().AsQueryable();
            }

            var final = (await query.OrderByDescending(j => j.DateComplete)
                .ToArrayAsync(cancellation))
                .Select(j => new
                {
                    DateComplete = j.DateComplete.ToUserLocal(),
                    j.JobId,
                    j.CustomerFileName,
                    j.InputFileName,
                    j.Product,
                    j.UserId,
                    TotalRecords = j.RecordCount,
                    MatchRecords = j.MatchCount,
                    j.UserName,
                    ProcessingTime =
                    (j.DateComplete - j.DateSubmitted).TotalMinutes < 1
                        ? "< 1"
                        : (j.DateComplete - j.DateSubmitted).TotalMinutes.ToString("N0"),
                    j.Source,
                    SourceDescription = j.Source.GetDescription(),
                    j.MatchRate,
                    Links = new
                    {
                        UserDetail = this.Url.BuildFor<UserDetailController>().ToDetail(j.UserId),
                        JobsForClient = this.Url.Action("Index", "Summary", new { Area = "JobProcessing", email = j.UserName }),
                        JobDetail = this.Url.Action("Index", "Detail", new { Area = "JobProcessing", jobId = j.JobId })
                    }
                }).OrderByDescending(o => o.DateComplete)
                .ToArray();


            var data = final.ToDataSourceResult(request);
            data.Total = final.Count();

            var jsonNetResult = new JsonNetResult(DateTimeKind.Local)
            {
                Data = data
            };
            return jsonNetResult;
        }


         /// <summary>
        /// Gets Jobs for a specific user
        /// </summary>
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public virtual async Task<ActionResult> Search([DataSourceRequest] DataSourceRequest request, Guid applicationId, string searchTerm, CancellationToken cancellation)
        {
            var startDate = DateTime.Now.AddDays(-60);
            var endDate = DateTime.Now;

            var query = this.dal.SearchDuring(applicationId, searchTerm, startDate, endDate);

            var final = (await query.OrderByDescending(j => j.DateComplete)
                .ToArrayAsync(cancellation))
                .Select(j => new
                {
                    DateComplete = j.DateComplete.ToUserLocal(),
                    j.JobId,
                    j.CustomerFileName,
                    j.InputFileName,
                    j.Product,
                    j.UserId,
                    TotalRecords = j.RecordCount,
                    MatchRecords = j.MatchCount,
                    j.UserName,
                    j.Source,
                    SourceDescription = j.Source.GetDescription(),
                    j.MatchRate,
                    Links = new
                    {
                        JobDetail = this.Url.Action("Index", "Summary", new { Area = "JobProcessing", jobId = j.JobId })
                    }
                })
                .ToArray();

            var data = final.ToDataSourceResult(request);
            data.Total = final.Count();

            var jsonNetResult = new JsonNetResult(DateTimeKind.Local)
            {
                Data = data
            };
            return jsonNetResult;
        }
    }
}