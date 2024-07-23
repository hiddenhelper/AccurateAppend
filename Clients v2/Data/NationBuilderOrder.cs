using System;
using AccurateAppend.Core;
using DomainModel.Enum;
using Integration.NationBuilder.Data;

namespace AccurateAppend.Websites.Clients.Data
{
    /// <summary>
    /// An integration job for NationBuilder lists.
    /// </summary>
    public class NationBuilderOrder : Order
    {
        protected NationBuilderOrder() { }

        public JobSource Source => JobSource.NationBuilder;

        public String Slug { get; protected set; }
        
        public PushStatus PushStatus { get; protected set; }

        public Int32 CurrentPage { get; protected set; }

        public override OrderType Type => OrderType.Push;

        public override String StatusDescription => this.DescribeStatus();

        public String SourceDescription => this.Source.GetDescription();

        public override Boolean CanDownload => this.PushStatus == PushStatus.Complete && this.DateSubmitted >= DateTime.UtcNow.AddDays(-29);

        public override String DownloadLink
        {
            get
            {
                if (!this.CanDownload) return null;

                return $"/JobProcessing/Download/Push/{this.Id}";
            }
        }

        public Boolean BillingComplete => this.PushStatus == PushStatus.Pushing;

    }
}