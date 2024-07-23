using System;

namespace AccurateAppend.Websites.Admin.Areas.Clients.UserFiles.Models
{
    public class FilesRequest
    {
        public Guid? UserId { get; set; }

        public String UserName { get; set; }

        public String PublicUploadLink { get; set; }

        public String UploadFileLink { get; set; }
    }
}