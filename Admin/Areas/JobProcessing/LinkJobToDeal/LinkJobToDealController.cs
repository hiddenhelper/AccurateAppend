using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing;
using AccurateAppend.JobProcessing.DataAccess;
using AccurateAppend.JobProcessing.Manifest.Xml;
using AccurateAppend.JobProcessing.Reporting;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Messages;
using AccurateAppend.Websites.Admin.Areas.JobProcessing.LinkJobToDeal.Models;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Areas.JobProcessing.LinkJobToDeal
{
    /// <summary>
    /// Controller to display linking a completed <see cref="Job"/> to an existing <see cref="DealBinder"/>.
    /// </summary>
    [Authorize()]
    public class LinkJobToDealController : Controller
    {
        #region Fields

        private readonly IMessageSession bus;
        private readonly DefaultContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <seealso cref="LinkJobToDealController"/> class.
        /// </summary>
        /// <param name="context">The <seealso cref="ISessionContext"/> to use.</param>
        /// <param name="bus">The <see cref="IMessageSession"/> providing bus access.</param>
        public LinkJobToDealController(DefaultContext context, IMessageSession bus)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (bus == null) throw new ArgumentNullException(nameof(bus));

            this.context = context;
            this.bus = bus;
        }

        #endregion

        #region Action Methods

        public virtual async Task<ActionResult> Display(Int32 jobId, CancellationToken cancellation)
        {
            var job = await this.EligibleJobs(jobId, cancellation);

            if (job == null) return this.DisplayErrorResult($"An error has occured while associating to the deal. The job {jobId} could not be found, is not a FTP/SMTP/Admin job, or is not {JobStatus.Complete}.");

            return this.View(job);
        }

        public virtual async Task<ActionResult> Select(Int32 jobId, Int32 dealId, CancellationToken cancellation)
        {
            var job = await this.context
                .SetOf<Job>()
                .Where(j => j.Id == jobId && j.Status == JobStatus.Complete)
                .Include(j => j.Processing)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellation);

            if (job == null) return this.DisplayErrorResult($"An error has occured while associating to the deal. The job {jobId} could not be found, is not a FTP/SMTP/Admin job, or is not {JobStatus.Complete}.");

            var command = new UpdateDealFromProcessingReportCommand
            {
                DealId = dealId,
                ProcessingReport = job.Processing.Report,
                PublicKey = job.PublicKey,
                Manifest = job.Manifest
            };
            command.Manifest.JobId(jobId);
            command.Manifest.CustomerFileName(job.CustomerFileName);

            await this.bus.Send(command);

            return this.View(job.PublicKey);
        }

        #endregion

        #region Helpers

        protected virtual Task<AssociateJobModel> EligibleJobs(Int32 jobId, CancellationToken cancellation)
        {
            const String Sql = @"
SELECT [JobId], [CustomerFileName], [PublicKey], [UserId], [UserName] FROM (
SELECT j.[JobId], [CustomerFileName], Convert(uniqueidentifier, [InputFileName]) [PublicKey], [UserId], [UserName], [Key]
FROM [admin].[JobQueueView] j
LEFT JOIN [jobs].[JobQueueLookup] l ON l.[JobId] = j.[JobId] AND l.[Key] = @p4
WHERE j.[Source] IN (@p0,@p1,@p2)
AND j.[Status]=@p3
AND j.[JobId]=@p5
) f
WHERE [Key] IS NULL";

            return this.context
                .Database
                .SqlQuery<AssociateJobModel>(
                    Sql,
                    (Int32) JobSource.Admin,
                    (Int32) JobSource.FTP,
                    (Int32) JobSource.SMTP,
                    (Int32) JobStatus.Complete,
                    LookupKey.AssociatedWithDeal.ToString(),
                    jobId)
                .FirstOrDefaultAsync(cancellation);
        }

        #endregion
    }
}