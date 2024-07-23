using System;

namespace AccurateAppend.Websites.Admin.Areas.Operations.Message.Models
{
    public class Attachment
    {
        public Boolean Exists { get; set; }
        public String  SendFileName { get; set; }

        public Boolean HasContentType => this.ContentType.Length > 0;

        public String ContentType { get; set; }
        public String FileName { get; set; }
    }
}