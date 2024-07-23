#pragma warning disable SA1402 // File may only contain a single class
using System;
using System.Data.Entity.ModelConfiguration;
using AccurateAppend.Core;

namespace AccurateAppend.Sales.ReadModel
{
    /// <summary>
    /// Readmodel for Deal Views.
    /// </summary>
    public class DealView
    {
        #region Fields

        private DateTime createdDate;
        private DateTime? completedDate;

        #endregion

        #region Constructor

        /// <summary>
        /// This is a readonly type.
        /// </summary>
        protected DealView()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier of the current deal.
        /// </summary>
        public virtual Int32 DealId { get; protected set; }

        /// <summary>
        /// Gets the identifier of the deal owner.
        /// </summary>
        public virtual Guid UserId { get; protected set; }

        /// <summary>
        /// Gets the identifier of the Deal application.
        /// </summary>
        public virtual Guid ApplicationId { get; protected set; }

        /// <summary>
        /// Gets the title of the application the deal is for.
        /// </summary>
        public String ApplicationName { get; protected set; }

        /// <summary>
        /// Gets the username of the Deal owner.
        /// </summary>
        public virtual String UserName { get; protected set; }

        /// <summary>
        /// Gets the amount of the current deal.
        /// </summary>
        public virtual Decimal Amount { get; protected set; }

        /// <summary>
        /// Gets the current status of the deal.
        /// </summary>
        public virtual DealStatus Status { get; protected set; }

        /// <summary>
        /// Gets the first name of the deal.
        /// </summary>
        public virtual String FirstName { get; protected set; }

        /// <summary>
        /// Gets the last name of the deal.
        /// </summary>
        public virtual String LastName { get; protected set; }

        /// <summary>
        /// Gets the business name of the deal.
        /// </summary>
        public virtual String BusinessName { get; protected set; }

        /// <summary>
        /// Gets the name for the deal.
        /// </summary>
        public virtual String Title { get; protected set; }

        /// <summary>
        /// Gets the description of the deal.
        /// </summary>
        public String Description { get; protected set; }

        /// <summary>
        /// Gets the instructions for the deal, if any.
        /// </summary>
        public String ProcessingInstructions { get; protected set; }

        /// <summary>
        /// Gets the date and time the deal was created.
        /// </summary>
        public virtual DateTime CreatedDate
        {
            get { return this.createdDate; }
            protected set { this.createdDate = value.Coerce(); }
        }

        /// <summary>
        /// Gets the date and time the deal was completed, if any.
        /// </summary>
        public virtual DateTime? CompleteDate
        {
            get { return this.completedDate; }
            protected set { this.completedDate = value.Coerce(); }
        }

        public Guid OwnerId { get; protected set; }

        public String OwnerName { get; protected set; }

        /// <summary>
        /// Indicates whether the current deal supports auto-billing.
        /// </summary>
        public Boolean EnableAutoBill { get; protected set; }

        /// <summary>
        /// Gets the unique public key for the current instance.
        /// </summary>
        /// <remarks>
        /// Public key values are aligned to other entities on other contexts via this common identifier.
        /// In example, a deal related to a file via a matching value.
        /// </remarks>
        /// <value>The unique public key for the current instance.</value>
        public Guid PublicKey { get; protected set; }

        #endregion
    }

    internal class DealViewConfiguration : EntityTypeConfiguration<DealView>
    {
        public DealViewConfiguration()
        {
            this.ToTable("DealsView", "sales");

            // Primary Key
            this.HasKey(c => c.DealId);
            
            this.Property(d => d.ApplicationId);
            this.Property(d => d.ApplicationName).IsUnicode(false).HasMaxLength(50).IsOptional();
            this.Property(d => d.UserName).IsUnicode().HasMaxLength(256);
            this.Property(d => d.FirstName).IsUnicode(false).HasMaxLength(100);
            this.Property(d => d.LastName).IsUnicode(false).HasMaxLength(100);
            this.Property(d => d.UserId);
            this.Property(d => d.Status);
            this.Property(d => d.Title).IsUnicode(false).HasMaxLength(150);
            this.Property(d => d.Description).IsUnicode(false).HasMaxLength(500);
            this.Property(d => d.ProcessingInstructions).IsUnicode(true).IsMaxLength();
            this.Property(d => d.BusinessName).IsUnicode(false).HasMaxLength(100).IsOptional();
            this.Property(d => d.Amount).HasPrecision(19, 4).IsRequired();
            this.Property(d => d.CreatedDate).HasColumnName("DateAdded");
            this.Property(d => d.CompleteDate).IsOptional().HasColumnName("DateComplete");
            this.Property(d => d.OwnerId);
            this.Property(d => d.OwnerName).IsUnicode().HasMaxLength(256);
            this.Property(d => d.PublicKey).IsRequired();
            this.Property(d => d.EnableAutoBill);
        }
    }
}
