using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing;
using AccurateAppend.JobProcessing.Reporting;
using DomainModel.ActionResults;
using DomainModel.Html;

namespace AccurateAppend.Websites.Clients.Areas.JobProcessing.MatchReport
{
    [Authorize()]
    public class Controller : System.Web.Mvc.Controller
    {
        #region Fields

        private readonly ISessionContext context;
        private readonly IReportFormatter reportformatter;
        private readonly IPdfGenerator generator;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> component providing data access to this instance.</param>
        /// <param name="reportformatter">The <see cref="IReportFormatter"/> component that can produce a formatted report for a job.</param>
        /// <param name="generator">The <see cref="IPdfGenerator"/> generation component.</param>
        public Controller(ISessionContext context, IReportFormatter reportformatter, IPdfGenerator generator)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (reportformatter == null) throw new ArgumentNullException(nameof(reportformatter));
            if (generator == null) throw new ArgumentNullException(nameof(generator));
            Contract.EndContractBlock();

            this.context = context;
            this.reportformatter = reportformatter;
            this.generator = generator;
        }

        #endregion

        #region Action Methods

        public async Task<ActionResult> Index(Guid id, CancellationToken cancellation)
        {
            using (this.context.CreateScope(ScopeOptions.ReadOnly))
            {
                var userId = this.User.Identity.GetIdentifier();

                var job = await this.context
                    .SetOf<Job>()
                    .Where(j => j.Owner.Id == userId)
                    .FirstOrDefaultAsync(j => j.PublicKey == id, cancellation);
                if (job == null)
                {
                    return new LiteralResult()
                    {
                        Data = $"Job {id} not found",
                        PreFormatted = true,
                        ContentType = MediaTypeNames.Text.Plain
                    };
                }
                
                var report = new BatchReport(job);

                var data = this.generator.FromHtml(reportformatter.Build(report));

                return this.File(data, MediaTypeNames.Application.Pdf, $"Processing Report - {job.CustomerFileName}.pdf");
            }
        }

        #endregion
    }
}