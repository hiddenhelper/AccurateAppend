using System;
using AccurateAppend.Core;
using AccurateAppend.Core.Definitions;
using DomainModel.Enum;

namespace AccurateAppend.Websites.Clients.Data
{
    /// <summary>
    /// A batch provided file append. That is, not an integration or client direct uploaded based job.
    /// </summary>
    public class BatchOrder : Order
    {
        public JobSource Source { get; protected set; }

        public JobStatus JobStatus { get; protected set; }
        
        public override OrderType Type => OrderType.Batch;

        public override String StatusDescription => this.DescribeStatus();

        public String SourceDescription => this.Source.GetDescription();
        
        public override Boolean CanDownload => this.JobStatus == JobStatus.Complete && this.DateSubmitted >= DateTime.Now.AddDays(-29);
    }
}