using System;
using System.Diagnostics.Contracts;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing;
using AccurateAppend.Websites.Admin.Controllers;
using DomainModel.ActionResults;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AccurateAppend.Websites.Admin.Areas.JobProcessing.DownloadFiles
{
    /// <summary>
    /// Controller for accessing <see cref="Job"/> files.
    /// </summary>
    [Authorize()]
    public class DownloadFilesController : ActivityLoggingController
    {
        #region Fields

        private readonly Plugin.Storage.AzureBlobStorageLocation rawFiles;
        private readonly IFileLocation outbox;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadFilesController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        /// <param name="rawCustomerFiles">The <see cref="Plugin.Storage.AzureBlobStorageLocation"/> used to access the raw customer files.</param>
        /// <param name="outbox">The <see cref="IFileLocation"/> used to provide access to the outbox.</param>
        public DownloadFilesController(ISessionContext context, Plugin.Storage.AzureBlobStorageLocation rawCustomerFiles, Plugin.Storage.AzureBlobStorageLocation outbox) : base(context)
        {
            if (rawCustomerFiles == null) throw new ArgumentNullException(nameof(rawCustomerFiles));
            if (outbox == null) throw new ArgumentNullException(nameof(outbox));
            Contract.EndContractBlock();

            this.rawFiles = rawCustomerFiles;
            this.outbox = outbox;
        }

        #endregion

        #region Actions

        public virtual async Task<ActionResult> Input(Int32 jobId)
        {
            using (this.Context.CreateScope(ScopeOptions.ReadOnly))
            {
                var job = await this.Context.SetOf<Job>().FirstOrDefaultAsync(j => j.Id == jobId);
                if (job == null) return DownloadResult.Empty;

                // Try the legacy naming approach first
                var rawFile = this.rawFiles.CreateBlobInstance(job.CustomerFileName);
                if (rawFile.Exists())
                {
                    rawFile.FileType = MediaTypeNames.Text.Plain;
                    var uri = rawFile.GetSharedAccessUri(SharedAccessBlobPermissions.Read).ToString();
                    return this.Redirect(uri);
                }
                
                // Fallback to the newer GUID approach
                rawFile = this.rawFiles.CreateBlobInstance(job.InputFileName);
                if (rawFile.Exists())
                {
                    rawFile.FileType = MediaTypeNames.Text.Plain;
                    var uri = rawFile.GetSharedAccessUri(DateTime.UtcNow.AddMinutes(10), SharedAccessBlobPermissions.Read, job.CustomerFileName).ToString();
                    return this.Redirect(uri);
                }

                return DownloadResult.Empty;
            }
        }

        // TODO: method returns blank page if file is missing
        public virtual async Task<ActionResult> Output(Int32 jobId, String fileName)
        {
            using (this.Context.CreateScope(ScopeOptions.ReadOnly))
            {
                var job = await this.Context.SetOf<Job>().FirstOrDefaultAsync(j => j.Id == jobId && j.Status == JobStatus.Complete);
                if (job == null) return DownloadResult.Empty;

                var outboxFile = (Plugin.Storage.AzureBlobFile)job.JobFiles().CsvOutputFile(this.outbox);
                if (outboxFile.Exists())
                {
                    outboxFile.FileType = MediaTypeNames.Text.Plain;
                    var uri = outboxFile.GetSharedAccessUri(DateTime.UtcNow.AddMinutes(10), SharedAccessBlobPermissions.Read, fileName ?? job.CustomerFileName).ToString();
                    return this.Redirect(uri);
                }
                
                return DownloadResult.Empty;
            }
        }

        #endregion
    }
}
