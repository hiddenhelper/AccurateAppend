using System;
using System.Web.Mvc;

namespace DomainModel.ActionResults
{
    /// <summary>
    /// Custom <see cref="ActionResult"/> used to download a file.
    /// </summary>
    /// <remarks>
    /// Code origionally from http://haacked.com/archive/2008/05/10/writing-a-custom-file-download-action-result-for-asp.net-mvc.aspx
    /// </remarks>
    public class DownloadResult : ActionResult
    {
        #region Fields

        private static readonly ActionResult EmptyValue = new EmptyResult();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadResult"/> class.
        /// </summary>
        /// <param name="virtualPath">The file path to send to the client.</param>
        public DownloadResult(string virtualPath)
        {
            this.VirtualPath = virtualPath;
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets or sets the path of the file to send to the client.
        /// </summary>
        /// <value>The path of the file to send to the client.</value>
        public String VirtualPath { get; private set; }

        /// <summary>
        /// Gets or sets the name of the file should be downloaded as.
        /// </summary>
        /// <value>the name of the file should be downloaded as.</value>
        public string FileDownloadName { get; set; }

        public string NewDownloadName { get; set; }

        /// <summary>
        /// Gets the <see cref="ActionResult"/> that should be used for a missing or empty file content.
        /// </summary>
        /// <value>The <see cref="ActionResult"/> that should be used for a missing or empty file content.</value>
        public static ActionResult Empty { get { return EmptyValue; }}

        #endregion

        #region Overrides
        
        public override void ExecuteResult(ControllerContext context)
        {
            if (!String.IsNullOrEmpty(this.FileDownloadName))
            {
                    context.HttpContext.Response.AddHeader("content-disposition", "attachment; filename=" + "\"" + (String.IsNullOrEmpty(this.NewDownloadName) ? this.FileDownloadName : this.NewDownloadName) + "\"");

                    context.HttpContext.Response.ContentType = MimeTypeHelper.ConvertMimeType(this.NewDownloadName);
            }
            var filePath = this.VirtualPath;
            context.HttpContext.Response.TransmitFile(filePath + "\\" + this.FileDownloadName);
        }

        #endregion

        #region Nested Types

        private sealed class EmptyResult : ActionResult
        {
            /// <summary>
            /// Enables processing of the result of an action method by a custom type that inherits from the <see cref="T:System.Web.Mvc.ActionResult"/> class.
            /// </summary>
            /// <param name="context">The context in which the result is executed. The context information includes the controller, HTTP content, request context, and route data.</param>
            public override void ExecuteResult(ControllerContext context)
            {
                context.HttpContext.Response.BinaryWrite(new Byte[0]);
            }
        }

        #endregion
    }
}
