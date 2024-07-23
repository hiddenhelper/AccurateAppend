using System;

namespace AccurateAppend.Websites.Clients.Areas.Order.Shared.Models
{
    /// <summary>
    /// Model used for presenting the client the ability to select a file to upload.
    /// </summary>
    [Serializable()]
    public class UploadRequestModel
    {
        /// <summary>
        /// Gets or sets the <see cref="Uri"/> that the file request should be uploaded to.
        /// </summary>
        public Uri UploadTo { get; set; }

        /// <summary>
        /// Gets or set the <see cref="Uri"/> that a box storage request should be made to.
        /// This value may be null which means Box.com integration is not supported in this instance.
        /// </summary>
        public Uri BoxUpload { get; set; }
    }
}