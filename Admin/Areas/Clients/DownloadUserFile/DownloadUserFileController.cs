using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Data;
using AccurateAppend.Operations;
using AccurateAppend.Operations.DataAccess;
using DomainModel.ActionResults;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AccurateAppend.Websites.Admin.Areas.Clients.DownloadUserFile
{
    /// <summary>
    /// Used to download files that were uploaded in the admin using the clients.xxx.com/net url
    /// </summary>
    [Authorize()]
    public class DownloadUserFileController : Controller
    {
        #region Fields

        private readonly DefaultContext context;
        private readonly Plugin.Storage.AzureBlobStorageLocation assistedFiles;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadUserFileController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        /// <param name="assistedFiles">The <see cref="Plugin.Storage.AzureBlobStorageLocation"/> holding the assisted storage files.</param>
        public DownloadUserFileController(DefaultContext context, Plugin.Storage.AzureBlobStorageLocation assistedFiles)
        {
            if (assistedFiles == null) throw new ArgumentNullException(nameof(assistedFiles));

            this.context = context;
            this.assistedFiles = assistedFiles;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Downloads a file by the system file name value.
        /// </summary>
        public virtual async Task<ActionResult> Index(String systemFileName, Guid? publicKey, CancellationToken cancellation)
        {
            var file = await this.context
                .Set<UserFile>()
                .Where(f => f.FileName == systemFileName || f.Id == publicKey)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellation);
            if (file == null) return DownloadResult.Empty;

            var content = this.assistedFiles.CreateBlobInstance(file.FileName);
            var accessUri = content.GetSharedAccessUri(DateTime.UtcNow.AddMinutes(10), SharedAccessBlobPermissions.Read, file.CustomerFileName);

            return this.Redirect(accessUri.ToString());
        }

        #endregion
    }
}