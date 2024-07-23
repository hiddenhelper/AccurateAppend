using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net.Mime;
using System.Web.Mvc;
using AccurateAppend.Core.Utilities;

namespace DomainModel.ActionResults
{
    /// <summary>
    /// Custom <see cref="ActionResult"/> that is designed to facilitate transmitting the
    /// contents of a <see cref="FileProxy"/> as a file transfer. Content transfer will
    /// be in raw binary data, no conversions (unless performed by the file instance itself)
    /// will be made to the data.
    /// </summary>
    public class FileProxyResult : ActionResult
    {
        #region Fields

        private readonly FileProxy file;
        private String fileDownloadName;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FileProxyResult"/> class.
        /// </summary>
        /// <param name="file">The <see cref="FileProxy"/> to transmit.</param>
        public FileProxyResult(FileProxy file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            Contract.EndContractBlock();

            this.file = file;
            if (!String.IsNullOrEmpty(file.FileType)) this.ContentType = file.FileType;
            this.FileDownloadName = file.Name;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the mime-type headers. that will be used when transfering the files.
        /// If the provided <see cref="File"/> has a <see cref="FileProxy.FileType"/> value,
        /// that will be used by default. A value that is explicitly set will ALWAYS take
        /// precidence over the value found on the file proxy.
        /// </summary>
        public String ContentType { get; set; }

        /// <summary>
        /// Gets the <see cref="FileProxy"/> to transmit the contents of.
        /// </summary>
        public FileProxy File
        {
            get
            {
                Contract.Ensures(Contract.Result<FileProxy>() != NullFile.Null);
                return this.file;
            }
        }

        /// <summary>
        /// Gets or sets the name of the file should be downloaded as via the "content-disposition"
        /// header. A null or empty value will always cause this to return to the underlying
        /// <see cref="FileProxy.Name"/> value.
        /// </summary>
        /// <value>The name of the file should be downloaded as via the "content-disposition" header.</value>
        public String FileDownloadName
        {
            get { return this.fileDownloadName; }
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    this.fileDownloadName = this.File.Name;
                }
                else
                {
                    this.fileDownloadName = value.Trim();
                }
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Enables processing of the result of an action method by a custom type that inherits from the <see cref="T:System.Web.Mvc.ActionResult"/> class.
        /// </summary>
        /// <param name="context">The context in which the result is executed. The context information includes the controller, HTTP content, request context, and route data.</param>
        public override void ExecuteResult(ControllerContext context)
        {
            var response = context.RequestContext.HttpContext.Response;

            if (this.File.Exists())
            {
                var buffer = new Byte[2048];

                using (var stream = this.File.OpenStream(FileAccess.Read))
                {
                    var dataLength = stream.Read(buffer, 0, buffer.Length);
                    while (dataLength > 0)
                    {
                        response.OutputStream.Write(buffer, 0, dataLength);
                        dataLength = stream.Read(buffer, 0, buffer.Length);
                    }
                }
            }
            else
            {
                response.BinaryWrite(new Byte[0]);
            }
            response.ContentType = String.IsNullOrWhiteSpace(this.ContentType)
                ? MediaTypeNames.Application.Octet
                : this.ContentType;

            response.AddHeader("content-disposition", "attachment; filename=" + "\"" + this.FileDownloadName + "\"");
        }

        #endregion
    }
}