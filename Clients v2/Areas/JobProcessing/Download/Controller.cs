using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.Plugin.Storage;
using AccurateAppend.Websites.Clients.Data;
using DomainModel.ActionResults;
using EventLogger;
using Integration.NationBuilder.Data;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AccurateAppend.Websites.Clients.Areas.JobProcessing.Download
{
    [Authorize()]
    public class DownloadController : Controller
    {
        #region Fields

        private readonly AzureBlobStorageLocation outbox;
        private readonly ISessionContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> component providing entity access.</param>
        /// <param name="outbox">The <see cref="AzureBlobStorageLocation"/> used to access the raw customer files.</param>
        public DownloadController(ISessionContext context, AzureBlobStorageLocation outbox)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (outbox == null) throw new ArgumentNullException(nameof(outbox));
            Contract.EndContractBlock();

            this.context = context;
            this.outbox = outbox;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Action to display submitted jobs for the interactive user.
        /// </summary>
        public virtual async Task<ActionResult> Push(Guid id)
        {
            try
            {
                using (this.context.CreateScope(ScopeOptions.ReadOnly))
                {
                    var query = this.context.SetOf<NationBuilderOrder>()
                        .ForInteractiveUser()
                        .Where(j => j.Id == id && j.PushStatus == PushStatus.Complete);

                    var job = await query.FirstOrDefaultAsync();
                    if (job == null) return DownloadResult.Empty;

                    try
                    {
                        var file = (AzureBlobFile)job.AccessOutputFile(this.outbox); // OK as we know the concrete file type
                        if (!file.Exists()) return new LiteralResult() { Data = "File no longer exists" };

                        file.FileType = MediaTypeNames.Text.Plain;
                        var uri = file.GetSharedAccessUri(DateTime.UtcNow.AddMinutes(10), SharedAccessBlobPermissions.Read, job.CustomerFileName).ToString();
                        return this.Redirect(uri);
                    }
                    catch (InvalidOperationException)
                    {
                        return DownloadResult.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogEvent(ex, Severity.Medium, Application.Clients, this.Request.UserHostAddress, $"Error downloading NB file for {id}");

                return DownloadResult.Empty;
            }
        }

        /// <summary>
        /// Action to display submitted jobs for the interactive user.
        /// </summary>
        public virtual async Task<ActionResult> Job(Guid id)
        {
            try
            {
                using (this.context.CreateScope(ScopeOptions.ReadOnly))
                {
                    var baseQuery = this.context.SetOf<Data.Order>()
                        .ForInteractiveUser()
                        .Where(j => j.Id == id);
                    var query1 = baseQuery.OfType<BatchOrder>().Where(j => j.JobStatus == JobStatus.Complete);
                    var query2 = baseQuery.OfType<DirectClientOrder>().Where(j => j.JobStatus == JobStatus.Complete);

                    var query = query1.Cast<Data.Order>().Concat(query2);

                    var job = await query.FirstOrDefaultAsync();
                    if (job == null) return DownloadResult.Empty;

                    try
                    {
                        var file = (AzureBlobFile)job.AccessOutputFile(this.outbox); // OK as we know the concrete file type
                        if (!file.Exists()) return DownloadResult.Empty;

                        file.FileType = MediaTypeNames.Text.Plain;
                        var uri = file.GetSharedAccessUri(DateTime.UtcNow.AddMinutes(10), SharedAccessBlobPermissions.Read, job.CustomerFileName).ToString();
                        return this.Redirect(uri);
                    }
                    catch (InvalidOperationException)
                    {
                        return DownloadResult.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogEvent(ex, Severity.Medium, Application.Clients, this.Request.UserHostAddress, $"Error downloading Job file for {id}");

                return DownloadResult.Empty;
            }
        }

        #endregion
    }
}