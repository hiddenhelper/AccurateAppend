using System;
using System.Collections.Generic;

namespace AccurateAppend.Websites.Admin.Areas.Operations.EventLog.Models
{
    [Serializable()]
    public class EventsRequest
    {
        public String CorrelationId { get; set; }

        public Int32? EventId { get; set; }

        public ICollection<String> Email { get; } = new List<String>();
    }
}