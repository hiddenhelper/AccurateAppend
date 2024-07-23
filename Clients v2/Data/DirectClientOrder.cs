using System;
using AccurateAppend.Core;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Sales;
using DomainModel.Enum;

namespace AccurateAppend.Websites.Clients.Data
{
    /// <summary>
    /// A manually uploaded file append from a client.
    /// </summary>
    public class DirectClientOrder : Order
    {
        public JobSource Source { get; protected set; }

        public JobStatus JobStatus { get; protected set; }
        
        public override OrderType Type => OrderType.Client;

        public override String StatusDescription => this.DescribeStatus();

        public String SourceDescription => this.Source.GetDescription();

        public override Boolean CanDownload
        {
            get
            {
                // Not complete or too old then NO
                if (this.OrderStatus != ProcessingStatus.Available) return false;
                if (this.DateSubmitted < DateTime.Now.AddDays(-29)) return false;
                
                return true;
            }
        }
    }
}