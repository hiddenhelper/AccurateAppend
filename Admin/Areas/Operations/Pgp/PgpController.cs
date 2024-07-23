using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Utilities;
using AccurateAppend.JobManagement.Contracts;
using AccurateAppend.Pgp;
using DomainModel;
using DomainModel.ActionResults;

namespace AccurateAppend.Websites.Admin.Areas.Operations.Pgp
{
    [Authorize()]
    public class PgpController : Controller
    {
        #region Fields

        private readonly IEncryptor encryption;
        private readonly IFileLocation temp;
        private readonly IKeyManager keyManager;

        #endregion

        #region Constructor

        public PgpController(IEncryptor encryption, IFileLocation temp, IKeyManager keyManager)
        {
            if (encryption == null) throw new ArgumentNullException(nameof(encryption));
            if (temp == null) throw new ArgumentNullException(nameof(temp));
            if (keyManager == null) throw new ArgumentNullException(nameof(keyManager));
            
            this.encryption = encryption;
            this.temp = temp;
            this.keyManager = keyManager;
        }

        #endregion

        #region Action Methods

        public ActionResult Upload()
        {
            var scheme = Uri.UriSchemeHttps;
#if DEBUG
            // If we're running in VS we need to use the http protocol so we override it here
            if (this.Request.Url.Host.EndsWith("localhost", StringComparison.OrdinalIgnoreCase)) scheme = Uri.UriSchemeHttp;
#endif

            var request = new UploadRequest(this.Url.Action(nameof(this.Decrypt), "Pgp", null, scheme));
            var uri = request.CreateRequest(this.encryption);
            
            return this.View(uri);
        }

        public async Task<ActionResult> Decrypt(CancellationToken cancellation)
        {
            var result = UploadResult.HandleFromPostback(this.Request.QueryString, this.encryption);

            String customerFileName;

            var rawFileName = JobPipeline.CleanFileName(Path.GetFileName(result.ClientFileName));
            if (Path.HasExtension(rawFileName))
            {
                customerFileName = Path.GetFileNameWithoutExtension(rawFileName);
            }
            else
            {
                customerFileName = Path.ChangeExtension(rawFileName, "csv");
            }
            
            var file = this.temp.CreateInstance(result.SystemFileName);

            if (!JobPipeline.IsPgp(file))
            {
                this.TempData["message"] = $"File with extension {Path.GetExtension(file.Name)} are not supported";
                return this.View("~/Views/Shared/Error.aspx");
            }

            var pgpFile = await this.keyManager.DecryptionWrapper(file, cancellation);
            return new FileProxyResult(pgpFile) {FileDownloadName = customerFileName};
        }

        #endregion
    }
}