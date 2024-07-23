using System;
using System.Collections.Generic;

namespace AccurateAppend.Websites.Admin.Entities
{
    [Serializable()]
    public class BatchJobRequest
    {
        public BatchJobRequest()
        {
            InputFile = new List<BatchJobRequestFile>();
        }

        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public int DealId { get; set; }
        public int JobId { get; set; }
        public string Category { get; set; }
        public BatchJobRequestProduct Product { get; set; }
        public string ClientReference { get; set; }
        public char? Delimiter { get; set; }
        public Guid ApplicationId { get; set; }
        public List<BatchJobRequestFile> InputFile { get; set; }
        public Guid ManifestId { get; set; }
        public bool HasHeaderRow { get; set; }
        public Uri PostbackUri { get; set; }
        public string Instructions { get; set; }
    }
}
