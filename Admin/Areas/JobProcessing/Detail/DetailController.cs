using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing;
using AccurateAppend.JobProcessing.Manifest.Xml;
using AccurateAppend.Websites.Admin.Areas.Clients.UserDetail;
using AccurateAppend.Websites.Admin.Areas.JobProcessing.DeleteJob;
using AccurateAppend.Websites.Admin.Areas.JobProcessing.Detail.Models;
using AccurateAppend.Websites.Admin.Areas.JobProcessing.LinkJobToDeal;
using AccurateAppend.Websites.Admin.Areas.JobProcessing.Reassign;
using AccurateAppend.Websites.Admin.Areas.JobProcessing.Reset;
using AccurateAppend.Websites.Admin.Areas.JobProcessing.Resume;
using AccurateAppend.Websites.Admin.Areas.JobProcessing.Review;
using AccurateAppend.Websites.Admin.Areas.Sales.NewDealFromJob;
using AccurateAppend.Websites.Admin.Controllers;
using AccurateAppend.Websites.Admin.Navigator;
using DomainModel.ActionResults;
using EntityFramework.Extensions;

namespace AccurateAppend.Websites.Admin.Areas.JobProcessing.Detail
{
    /// <summary>
    /// Controller for presenting job processing information
    /// </summary>
    [Authorize()]
    public class DetailController : ContextBoundController
    {
        #region Fields

        private static readonly PropertyInfo Property = typeof(Job).GetProperty("ProcessingCost", BindingFlags.GetProperty | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.NonPublic);

        #endregion

        #region Constants

        private const String SourceAdmin = "Admin";
        private const String SourceFtp = "FTP";
        private const String SourceSmtp = "SMTP";
        private const String SourceNationBuilder = "NB";
        private const String SourceClients = "Client";
        private const String SourceListbuilder = "LB";

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        public DetailController(ISessionContext context) : base(context)
        {
        }

        #endregion

        #region Action Methods

        [OutputCache(Duration = 0, VaryByParam = "None")]
        public ActionResult Index(Int32 jobid)
        {
            // TODO: let's port over to the readmodel
            var data = new JobDetail();

            var jobs = this.Context.SetOf<Job>().Where(j => j.Id == jobid);
            var report = jobs.Select(j => j.Processing).FutureFirstOrDefault();
            var job = this.Context.SetOf<Job>().Include(j => j.Lookups).FirstOrDefault(j => j.Id == jobid);
            if (job != null)
            {
                data.CustomerFileName = job.CustomerFileName;
                data.DateComplete = job.DateCompleted.Value.ToUserLocal();
                data.DateSubmitted = job.DateSubmitted.ToUserLocal();
                data.DateUpdated = job.DateUpdated.ToUserLocal();

                data.InputFileSize = job.FileSize;
                data.TotalRecords = job.TotalRecords;
                data.ProcessedRecords = report.Value.ProcessedRecords;
                data.MatchRecords = report.Value.MatchedRecords;
                data.SystemErrors = report.Value.SystemErrors;

                data.JobId = job.Id.Value;
                data.Status = job.Status;
                data.Priority = (Int32)job.Priority;
                data.UserId = job.Owner.Id;
                data.UserName = job.Owner.UserName;
                data.InputFileName = job.InputFileName;
                data.Product = String.Join(";", job.Manifest.Operations().Select(o => o.OperationName().ToString()));

                data.Message = job.AccessLookups().ErrorMessage;
                data.SensitiveData = job.AccessLookups().SensitiveData;
                data.DealId = job.AccessLookups().AssociatedWithDeal;
                data.IsPaused = ((Int32?) Property.GetValue(job) == null);

                if (job is ManualJob)
                {
                    data.Source = SourceAdmin;
                }
                else if (job is FtpJob)
                {
                    data.PublicFtpFolder = job.Lookups.FirstOrDefault(a => a.Key == LookupKey.FtpDirectoryPublic)?.Value;
                    data.Source = SourceFtp;
                }
                else if (job is SmtpJob)
                {
                    data.AltEmail = job.Lookups.FirstOrDefault(a => a.Key == LookupKey.AltEmail)?.Value;
                    data.Source = SourceSmtp;
                }
                else if (job is IntegrationJob)
                {
                    data.Source = SourceNationBuilder;
                }
                else if (job is ClientJob)
                {
                    data.Source = SourceClients;
                }
                else if (job is ListbuilderJob)
                {
                    data.Source = SourceListbuilder;
                }
            }

            var result = new JsonNetResult
            {
                Data = new
                {
                    data.InputFileName,
                    data.CustomerFileName,
                    data.DateComplete,
                    data.DateSubmitted,
                    data.DateUpdated,
                    data.InputFileSize,
                    data.JobId,
                    data.MatchRecords,
                    data.Message,
                    Status = (Int32)data.Status,
                    StatusDescription = data.StatusDescription + (job.IsPaused() ? " (paused)" : String.Empty),
                    data.PublicFtpFolder,
                    data.AltEmail,
                    data.Source,
                    data.Priority,
                    data.ProcessedRecords,
                    data.Product,
                    data.TotalRecords,
                    data.UserId,
                    data.UserName,
                    data.SystemErrors,
                    data.FileSizeDescription,
                    Links = new
                    {
                        Detail = this.DetailLink(data),
                        UserDetail = this.UserLink(data),
                        SliceDetail = this.SliceDetailLink(data),
                        MatchLevelReport = this.MatchLevelReportLink(data),
                        MatchTypeReport = this.MatchTypeReportLink(data),
                        DownloadMatchLevelReport = this.DownloadMatchLevelReportLink(data),
                        DownloadMatchTypeReport = this.DownloadMatchTypeReportLink(data),
                        DownloadManifest = this.DownloadManifestLink(data),
                        ViewManifest = this.ViewManifestLink(data),
                        ViewDeal = this.ViewDealLink(data)
                    },
                    Actions = new
                    {
                        Remap = this.RemapLink(data),
                        Reset = this.ResetLink(data),
                        Resume = this.ResumeLink(data),
                        Review = this.ReviewLink(data),
                        Reassign = ReassignLink(data),
                        Delete = this.DeleteLink(data),
                        Pause = this.PauseLink(data),
                        NewDeal = this.NewDealLink(data),
                        ExistingDeal = this.ExistingDealLink(data)
                    }
                }
            };
            return result;
        }

        #endregion

        #region Link Helpers

        public String DetailLink(JobDetail data)
        {
            return this.Url.Action("Index", "Summary", new {Area = "JobProcessing", jobId = data.JobId}, Uri.UriSchemeHttps);
        }

        public String UserLink(JobDetail data)
        {
            return this.Url.BuildFor<UserDetailController>().ToDetail(data.UserId);
        }

        public String SliceDetailLink(JobDetail data)
        {
            return this.Url.Action("Index", "Dashboard", new {Area = "JobProcessing", data.JobId});
        }

        public String MatchLevelReportLink(JobDetail data)
        {
            return data.Status == JobStatus.Complete
                ? this.Url.Action("Index", "MatchReport", new {Area = "JobProcessing", data.JobId, type = "plainText"})
                : null;
        }

        public String MatchTypeReportLink(JobDetail data)
        {
            return data.Status == JobStatus.Complete
                ? this.Url.Action("Index", "MatchReport", new {Area = "JobProcessing", data.JobId, type = "plainText (new)"})
                : null;
        }

        public String DownloadMatchLevelReportLink(JobDetail data)
        {
            return data.Status == JobStatus.Complete
                ? this.Url.Action("Download", "MatchReport", new {Area = "JobProcessing", data.JobId, type = "plainText"})
                : null;
        }

        public String DownloadMatchTypeReportLink(JobDetail data)
        {
            return data.Status == JobStatus.Complete
                ? this.Url.Action("Download", "MatchReport", new {Area = "JobProcessing", data.JobId, type = "plainText (new)"})
                : null;
        }

        public String DownloadManifestLink(JobDetail data)
        {
            return this.Url.Action("Download", "Manifest", new {Area = "JobProcessing", data.JobId});
        }

        public String ViewManifestLink(JobDetail data)
        {
            return this.Url.Action("View", "Manifest", new {Area = "JobProcessing", data.JobId});
        }

        public String ViewDealLink(JobDetail data)
        {
            return !data.IsAssociated
                ? null
                : this.Url.Action("Index", "DealDetail", new {Area = "Sales", data.DealId});
        }

        public String RemapLink(JobDetail data)
        {
            if (data.Source == SourceNationBuilder) return null;
            return (!data.SensitiveData) || (data.SensitiveData && data.Status != JobStatus.Complete)
                ? this.Url.Action("UpdateColumnMap", "Batch", new {Area = "", jobId = data.JobId})
                : null;
        }

        public String ResetLink(JobDetail data)
        {
            return this.Url.BuildFor<ResetController>().Reset(data.JobId);
        }

        public String ResumeLink(JobDetail data)
        {
            return data.Status == JobStatus.Failed
                ? this.Url.BuildFor<ResumeController>().Resume(data.JobId)
                : data.IsPaused
                    ? this.Url.Action("Undo", "Pause", new { Area = "JobProcessing", jobId = data.JobId })
                    : null;
        }

        public String ReviewLink(JobDetail data)
        {
            return data.Status == JobStatus.Failed || data.Status == JobStatus.NeedsReview
                ? this.Url.BuildFor<ReviewController>().ReviewJob(data.JobId)
                : null;
        }

        public String DeleteLink(JobDetail data)
        {
            if (data.Source == SourceNationBuilder) return null;
            return this.Url.BuildFor<DeleteJobController>().Delete(data.JobId);
        }

        public String ReassignLink(JobDetail data)
        {
            if (data.Source == SourceNationBuilder || data.Source == SourceClients) return null;
            return this.Url.BuildFor<ReassignController>().Reassign(data.JobId);
        }

        public String PauseLink(JobDetail data)
        {
            return data.Status == JobStatus.Complete || data.IsPaused
                ? null
                : this.Url.Action("Index", "Pause", new {Area = "JobProcessing", jobId = data.JobId});
        }

        public String NewDealLink(JobDetail data)
        {
            if (data.Source == SourceClients || data.Source == SourceNationBuilder || data.Source == SourceListbuilder) return null;
            return data.Status == JobStatus.Complete && !data.IsAssociated
                ? this.Url.BuildFor<NewDealFromJobController>().FromJob(data.JobId)
                : null;
        }

        public String ExistingDealLink(JobDetail data)
        {
            if (data.Source == SourceClients || data.Source == SourceNationBuilder || data.Source == SourceListbuilder) return null;

            return data.Status == JobStatus.Complete && !data.IsAssociated
                ? this.Url.BuildFor<LinkJobToDealController>().AssociateWithJob(data.JobId)
                : null;
        }

        #endregion
    }
}