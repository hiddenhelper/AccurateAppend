using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.Utilities;
using AccurateAppend.JobManagement.Contracts;
using AccurateAppend.Plugin.Storage;
using AccurateAppend.Websites.Storage.Models;
using DomainModel;
using DomainModel.ActionResults;
using EventLogger;

namespace AccurateAppend.Websites.Storage.Controllers
{
    /// <summary>
    /// Controller for accepting upload requests from remote AA systems.
    /// </summary>
    public class UploadController :Controller
    {
        #region Fields

        private readonly IFileLocation temp;
        private readonly IFileLocation rawCustomerFiles;
        private readonly IEncryptor encryption;

        #endregion

        #region Constructor

        public UploadController(IFileLocation temp, IFileLocation rawCustomerFiles, IEncryptor encryption)
        {
            if (temp == null) throw new ArgumentNullException(nameof(temp));
            if (rawCustomerFiles == null) throw new ArgumentNullException(nameof(rawCustomerFiles));
            if (encryption == null) throw new ArgumentNullException(nameof(encryption));
            Contract.EndContractBlock();

            this.temp = temp;
            this.rawCustomerFiles = rawCustomerFiles;
            this.encryption = encryption;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Debug utility used to decrypt an encrypted storage app querystring request.
        /// </summary>
        public ActionResult Decrypt(String token)
        {
            return new LiteralResult(true) {Data = this.encryption.SymetricDecrypt(token)};
        }

        /// <summary>
        /// Presents a view to allow file section and submission to the storage application.
        /// </summary>
        public ActionResult Index()
        {
            // In debug use=> ?Token=3p2Sbz3J9AIhMf4iQtIBQM%2f%2f9m1%2ftJ6pRwKLz0NmjbNfqxRZUur%2f5PIG7MSNtnBQNdwQBHrEVD6nzhHCNtpW07B9Z%2bpydxCSvhvwhtIVF4V4n5RwJLgszC1NHzV80pvvJ7WRsO8USbbn5AUrvWh3Dkzmz7p2C5UQOMIFsxLmeGs%3d

            var request = UploadRequest.HandleToken(this.Request.QueryString, this.encryption);
            
            this.Session["Upload Request"] = request;

            return this.View(new UploadModel()
                {
                    Fail = this.Url.Action("Fail"),
                    Success = this.Url.Action("Success")
                });
        }
        
        /// <summary>
        /// Action to process posted files.
        /// </summary>
        [AcceptVerbs(HttpVerbs.Post)]
        [CorsHandler()]
        public virtual async Task<ActionResult> Save(IEnumerable<HttpPostedFileBase> files, Boolean? supportRedirect,  CancellationToken cancellation)
        {
            files = files?.ToArray();
            supportRedirect = supportRedirect ?? false;

            Logger.LogEvent("Upload started", Severity.None, Application.Clients, this.Request.UserHostAddress, Request.QueryString["Token"]);

            UploadRequest request;

            try
            {
                request = this.Session["Upload Request"] as UploadRequest ??
                          UploadRequest.HandleToken(this.Request.QueryString, this.encryption);

                if (request == null)
                {
                    if (supportRedirect.Value)
                    {
                        this.TempData["message"] = "Your request has timed out.";
                        return this.Redirect("Error.aspx");
                    }

                    return this.Json(new {status = (Int32) HttpStatusCode.RequestTimeout, message = "Your request has timed out"}, MediaTypeNames.Text.Plain);
                }
            }
            catch (Exception ex)
            {
                Logger.LogEvent(ex, Severity.High, Request.UserHostAddress);
                return this.Json(new { status = (Int32)HttpStatusCode.BadRequest, message = "Your request could not be understood", data = ex.Message }, MediaTypeNames.Text.Plain);
            }

            using (new Correlation(request.Identifier))
            {
                try
                {
                    #region Validation

                    Uri redirectTo;
                    UploadResult response;
                    if (files == null || !files.Any())
                    {
                        response = UploadResult.Canceled(request.Identifier, request.CorrelationId);
                        redirectTo = response.CreatePostback(request, this.encryption);

                        if (supportRedirect.Value) return this.Redirect(redirectTo.ToString());

                        Logger.LogEvent("No files uploaded", Severity.High, Application.Clients, Request.UserHostAddress, redirectTo.ToString());
                        return this.Json(new { status = (Int32)HttpStatusCode.BadRequest, message = "No files uploaded", data = redirectTo.ToString() }, MediaTypeNames.Text.Plain);
                    }

                    #endregion

                    var file = files.First();
                    var origionalFileName = JobPipeline.CleanFileName(file.FileName);
                    
                    Logger.LogEvent($"Client filename cleaned to {origionalFileName}", Severity.None, Application.Clients, Request.UserHostAddress);

                    var storage = this.temp.CreateInstance($"{request.Identifier}{Path.GetExtension(origionalFileName)}");
                    using (var writeTo = storage.OpenStream(FileAccess.Write, true))
                    {
                        await file.InputStream.CopyToAsync(writeTo, cancellation);
                    }

                    var conversionResult = await this.ConvertToCsvAsync(request, storage, origionalFileName, cancellation);
                    
                    origionalFileName = conversionResult.CustomerFileName;
                    storage = conversionResult.Convertedfile;
                    
                    Logger.LogEvent($"Client file stored at {storage}", Severity.None, Application.Clients, Request.UserHostAddress);

                    response = new UploadResult(request.Identifier, storage.Name, storage.Path, origionalFileName)
                    {
                        CorrelationId = request.CorrelationId
                    };

                    redirectTo = response.CreatePostback(request, this.encryption);

                    if (supportRedirect.Value) return this.Redirect(redirectTo.ToString());

                    return this.Json(new { status = (Int32)HttpStatusCode.OK, data = redirectTo.ToString() }, MediaTypeNames.Text.Plain);

                }
                catch (ExcelFormatException ex)
                {
                    Logger.LogEvent(ex, Severity.Low, request.Identifier.ToString());

                    return this.Json(new { status = (Int32)HttpStatusCode.BadRequest, message = ex.Message, data = String.Empty }, MediaTypeNames.Text.Plain);
                }
                catch (Exception ex)
                {
                    Logger.LogEvent(ex, Severity.Medium, request.Identifier.ToString());

                    return this.Json(new { status = (Int32)HttpStatusCode.InternalServerError, message = "Error uploading your file", data = String.Empty }, MediaTypeNames.Text.Plain);
                }
            }
        }

        #endregion

        #region Utilties

        protected virtual async Task<FileConversionResult> ConvertToCsvAsync(UploadRequest request, FileProxy storage, String origionalFileName, CancellationToken cancellation)
        {
            try
            {
                if (request.ConvertToCsv && JobPipeline.IsExcel(storage))
                {
                    var format = ExcelFile.DetermineExcelFormat(storage);
                    var excelContent = new ExcelFile(format, storage);
                    var convertedFile = await JobPipeline.HandleExcel(excelContent, request.Identifier, this.temp, this.rawCustomerFiles, cancellation).ConfigureAwait(false);
                    storage = convertedFile;

                    origionalFileName = Path.ChangeExtension(origionalFileName, Path.GetExtension(convertedFile.Name));
                }

                return new FileConversionResult() {Convertedfile = storage, CustomerFileName = origionalFileName};
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached) Debugger.Break();
                throw new ExcelFormatException("The file is an invalid format or is not an excel file", ex);
            }
        }

        [DebuggerDisplay("{" + nameof(CustomerFileName) + "}, {" + nameof(Convertedfile) + "}")]
        public struct FileConversionResult
        {
            public FileProxy Convertedfile { get; set; }

            public String CustomerFileName { get; set; }
        }

        #endregion
    }
}