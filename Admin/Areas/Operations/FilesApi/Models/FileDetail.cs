using System;

namespace AccurateAppend.Websites.Admin.Areas.Operations.FilesApi.Models
{
    [Serializable()]
    public class FileDetail
    {
        public Guid ApplicationId { get; set; }

        public Guid UserId { get; set; }

        public Guid FileId { get; set; }

        public String UserName { get; set; }

        public Int32 FileSize { get; set; }

        public String CustomerFileName { get; set; }

        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// The identifier that can be used to group or correlate this stored file to other stored files.
        /// </summary>
        public virtual Guid? CorrelationId { get; set; }

        /// <summary>
        /// Gets the system stored name of the file.
        /// </summary>
        /// <value>The system stored name of the file.</value>
        public virtual String SystemFileName { get; set; }
    }
}