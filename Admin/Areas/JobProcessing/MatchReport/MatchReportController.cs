using System;
using System.Diagnostics.Contracts;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing;
using AccurateAppend.JobProcessing.Reporting;
using AccurateAppend.Websites.Admin.Controllers;
using DomainModel.ActionResults;
using DomainModel.Html;

namespace AccurateAppend.Websites.Admin.Areas.JobProcessing.MatchReport
{
    /// <summary>
    /// Controller for rendering a job processing report.
    /// </summary>
    [Authorize()]
    public class MatchReportController : ContextBoundController
    {
        #region Fields

        private readonly IPdfGenerator generator;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchReportController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        /// <param name="generator">The <see cref="IPdfGenerator"/> component used for PDF creation.</param>
        public MatchReportController(ISessionContext context, IPdfGenerator generator) : base(context)
        {
            if (generator == null) throw new ArgumentNullException(nameof(generator));
            Contract.EndContractBlock();

            this.generator = generator;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// In-browser version of match report.
        /// </summary>
        public virtual async Task<ActionResult> Index(Int32 jobid, String type, CancellationToken cancellation)
        {
            using (this.Context.CreateScope(ScopeOptions.ReadOnly))
            {
                var job = await this.FindJob(jobid, cancellation);
                if (job == null) return this.View($"Job {jobid} not found");

                var formatter = this.CreateFormatter(type);

                var report = new BatchReport(job);

                var result = new LiteralResult {Title = "report", Data = formatter.Build(report)};
                return result;
            }
        }

        /// <summary>
        /// Downloadable version of match report
        /// </summary>
        public virtual async Task<ActionResult> Download(Int32 jobid, String type, CancellationToken cancellation)
        {
            using (this.Context.CreateScope(ScopeOptions.ReadOnly))
            {
                var job = await this.FindJob(jobid, cancellation);
                if (job == null) return this.View($"Job {jobid} not found");

                var formatter = this.CreateFormatter(type);

                var report = new BatchReport(job);

                var data = this.generator.FromHtml(formatter.Build(report));
                
                return this.File(data, MediaTypeNames.Application.Pdf, $"Processing Report - {job.CustomerFileName}.pdf");
            }
        }

        #endregion

        #region Helpers

        protected virtual Task<Job> FindJob(Int32 jobId, CancellationToken cancellation)
        {
            return this.Context.SetOf<Job>().FirstOrDefaultAsync(j => j.Id == jobId, cancellation);
        }

        protected virtual IReportFormatter CreateFormatter(String type)
        {
            var grouping = type == "plainText (new)"
                ? TextReportFormatter.Grouping.MatchType
                : TextReportFormatter.Grouping.MatchLevel;

            return new TextReportFormatter(grouping);
        }

        #endregion
    }
}
