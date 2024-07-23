using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing;
using AccurateAppend.JobProcessing.Manifest;
using AccurateAppend.JobProcessing.Manifest.Xml;
using AccurateAppend.Websites.Admin.Filters;

namespace AccurateAppend.Websites.Admin.Areas.JobProcessing.Pause
{
    /// <summary>
    /// Controller for pausing execution of a job.
    /// </summary>
    [Authorize()]
    public class PauseController : Controller
    {
        #region Fields

        private readonly AccurateAppend.JobProcessing.DataAccess.DefaultContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PauseController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="AccurateAppend.JobProcessing.DataAccess.DefaultContext"/> to use for this controller instance.</param>
        public PauseController(AccurateAppend.JobProcessing.DataAccess.DefaultContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region Actions

        [HandleErrorWithAjaxFilter()]
        public async Task<ActionResult> Index(Int32 jobid, CancellationToken cancellation)
        {
            try
            {
                await this.context.Database
                    .ExecuteSqlCommandAsync(
                        $"UPDATE [jobs].[JobQueue] SET [ProcessingCost]=null WHERE [JobId]=@p0 AND [Status] NOT IN ({(Int32)JobStatus.Complete}, {(Int32)JobStatus.WaitingToBeRecombined})",
                        cancellation,
                        jobid);

                return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return this.Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HandleErrorWithAjaxFilter()]
        public async Task<ActionResult> Undo(Int32 jobid, CancellationToken cancellation)
        {
            try
            {
                var job = await this.context.SetOf<Job>().SingleOrDefaultAsync(j => j.Id == jobid, cancellation);
                if (job != null)
                {

                    var processingCost = job.Manifest.Operations()
                        .Select(o => o.OperationName())
                        .Max(o => o.EstimateProcessingCost());
                
                    await this.context.Database
                        .ExecuteSqlCommandAsync(
                            $"UPDATE [jobs].[JobQueue] SET [ProcessingCost]=@p1 WHERE [JobId]=@p0 AND [Status] NOT IN ({(Int32)JobStatus.Complete}, {(Int32)JobStatus.WaitingToBeRecombined})",
                            cancellation,
                            jobid,
                            processingCost);
                }

                return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return this.Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}
