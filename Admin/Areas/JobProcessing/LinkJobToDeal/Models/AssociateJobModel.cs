using System;

namespace AccurateAppend.Websites.Admin.Areas.JobProcessing.LinkJobToDeal.Models
{
    [Serializable()]
    public class AssociateJobModel
    {
        public Int32 JobId { get; set; }

        public String CustomerFileName { get; set; }

        public Guid PublicKey { get; set; }

        public Guid UserId { get; set; }

        public String UserName { get; set; }
    }
}