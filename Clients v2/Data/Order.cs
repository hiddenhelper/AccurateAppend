using System;
using AccurateAppend.Core;
using AccurateAppend.Core.Utilities;
using AccurateAppend.Sales;

namespace AccurateAppend.Websites.Clients.Data
{
    public abstract class Order
    {
        private DateTime dateSubmitted;
        
        public Guid Id { get; protected set; }

        public Int32? RequestId { get; protected set; }

        public Guid UserId { get; protected set; }
        
        public DateTime DateSubmitted
        {
            get => this.dateSubmitted;
            protected set => this.dateSubmitted = value.Coerce();
        }
        
        public String Name { get; protected set; }

        public String CustomerFileName { get; protected set; }

        public String SystemFileName { get; protected set; }

        public Int32 TotalRecords { get; protected set; }

        public abstract OrderType Type { get; }

        /// <summary>
        /// Gets the current status in completing the client public order.
        /// </summary>
        public virtual ProcessingStatus OrderStatus { get; protected set; }

        /// <summary>
        /// Gets the public key of the bill that the order generated, id any.
        /// </summary>
        public virtual Guid? BillId { get; protected set; }

        public abstract String StatusDescription { get; }

        public abstract Boolean CanDownload { get; }

        public virtual String DownloadLink
        {
            get
            {
                if (!this.CanDownload) return null;

                return $"/JobProcessing/Download/Job/{this.Id}";
            }
        }

        public virtual String ReportDownloadLink
        {
            get
            {
                if (!this.CanDownload) return null;

                return $"/JobProcessing/MatchReport/Index/{this.Id}";
            }
        }

        public virtual String ReceiptDownloadLink
        {
            get
            {
                if (this.BillId == null) return null;

                return $"/Order/Bills/Receipt/{this.BillId}";
            }
        }

        public FileProxy AccessOutputFile(IFileLocation outbox)
        {
            if (!this.CanDownload) throw new InvalidOperationException($"Cannot download file for order {this.Id} as it is disallowed due to age or status");
            
            return outbox.CreateInstance($"{this.SystemFileName.ToLowerInvariant()}.csv");
        }
    }
}