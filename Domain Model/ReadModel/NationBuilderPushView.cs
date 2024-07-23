using System;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Xml.Linq;
using AccurateAppend.Core;
using AccurateAppend.Data;
using Integration.NationBuilder.Data;

namespace DomainModel.ReadModel
{
    [Serializable()]
    public class NationBuilderPushView
    {
        #region Fields

        private DateTime requestDate;
        private DateTime? waitTill;

        #endregion

        #region Constructor

        /// <summary>
        /// This type is readonly.
        /// </summary>
        protected NationBuilderPushView()
        {
        }

        #endregion

        #region Properties

        public Int32 Id { get; protected set; }

        public String UserName { get; protected set; }

        public Guid UserId { get; protected set; }

        public String Slug { get; protected set; }

        public DateTime RequestDate
        {
            get => this.requestDate;
            protected set => this.requestDate = value.Coerce();
        }

        public PushStatus Status { get; protected set; }

        public String StatusDescription
        {
            get
            {
                if (this.Status == PushStatus.Pushing && this.LockId != null) return "Uploading";
                if ((this.Status == PushStatus.Pending || this.Status == PushStatus.Acquired) && this.LockId != null)
                    return "Downloading";
                if (this.Status == PushStatus.Pushing && this.LockId == null) return "Billing";
                return this.Status.ToString();
            }
        }

        public Int32 ErrorsEncountered { get; protected set; }

        public Int32 TotalRecords { get; protected set; }

        public Int32 TotalPages { get; protected set; }

        public Int32 CurrentPage { get; protected set; }

        public Boolean CanResume
        {
            get
            {
                if (this.Status == PushStatus.Failed || this.Status == PushStatus.Review || (this.Status == PushStatus.Pushing && this.WaitTill > DateTime.UtcNow))
                {
                    return true;
                }

                return false;
            }
        }

        public Boolean CanCancel
        {
            get
            {
                if (this.Status != PushStatus.Complete && (this.DealStatus == null || this.DealStatus.Value != AccurateAppend.Accounting.DealStatus.Complete))
                {
                    return true;
                }
                return false;
            }
        }

        public Guid CorrelationId { get; protected set; }

        protected String Instructions { get; set; }

        public Guid? LockId { get; protected set; }

        public AccurateAppend.Accounting.DealStatus? DealStatus { get; protected set; }

        public DateTime? WaitTill
        {
            get { return this.waitTill; }
            protected set { this.waitTill = value?.Coerce(); }
        }

        public Int32? JobId { get; protected set; }

        public String ListName { get; protected set; }

        public String Product
        {
            get
            {
                var result = XElement.Parse(this.Instructions)
                    .Descendants(XName.Get("ProcessingInstruction", "http://schemas.datacontract.org/2004/07/Integration.NationBuilder.Data"))
                    .SelectMany(e=>e.Descendants(XName.Get("ProductKey", "http://schemas.datacontract.org/2004/07/Integration.NationBuilder.Data")))
                    .Select(e => e.Value)
                    .Distinct()
                    .ToArray();

                return String.Join(";", result);
            }
        }

        #endregion
    }

    public class NationBuilderPushViewConfiguration : EntityTypeConfiguration<NationBuilderPushView>
    {
        public NationBuilderPushViewConfiguration()
        {
            this.ToTable("NationBuilderPushView", "admin");

            this.Ignore(p => p.CanCancel);
            this.Ignore(p => p.CanResume);
            this.Ignore(p => p.StatusDescription);
            this.Ignore(p => p.Product);

            this.HasKey(p => p.Id);
            this.Property(p => p.CorrelationId).IsRequired();
            this.Property(p => p.CurrentPage).IsRequired();
            this.Property(p => p.DealStatus).IsOptional();
            this.Property(p => p.Id).IsRequired();
            this.Property("Instructions").IsRequired();
            this.Property(p => p.ErrorsEncountered).IsRequired();
            this.Property(p => p.JobId).IsOptional();
            this.Property(p => p.ListName).IsRequired();
            this.Property(p => p.LockId).IsOptional();
            this.Property(p => p.Slug).IsRequired();
            this.Property(p => p.Status).IsRequired();
            this.Property(p => p.TotalPages).IsRequired();
            this.Property(p => p.TotalRecords).IsRequired();
            this.Property(p => p.UserId).IsRequired();
            this.Property(p => p.UserName).IsRequired();
            this.Property(p => p.WaitTill).IsOptional();
        }
    }
}
