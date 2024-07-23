using System;
using System.Data.Entity.ModelConfiguration;
using AccurateAppend.Accounting;
using AccurateAppend.Core;

namespace DomainModel.ReadModel
{
    /// <summary>
    /// Readmodel for Lead Views.
    /// </summary>
    public class LeadView
    {
        #region Constructor

        /// <summary>
        /// This is a readonly type.
        /// </summary>
        protected LeadView()
        {
        }

        #endregion

        #region Fields

        private DateTime lastUpdate;
        private DateTime? followUpDate;
        private DateTime dateAdded;

        #endregion

        #region Properties

        public Int32 LeadId { get; protected set; }

        public String FirstName { get; protected set; }

        public String LastName { get; set; }

        public String BusinessName { get; protected set; }

        public String Email { get; protected set; }

        public String Phone { get; protected set; }

        public Int32 NoteCount { get; protected set; }

        public Boolean DoNotMarketTo { get; protected set; }

        public DateTime LastUpdate
        {
            get { return this.lastUpdate; }
            protected set { this.lastUpdate = value.Coerce(); }
        }

        public DateTime? FollowUpDate
        {
            get { return this.followUpDate; }
            protected set
            {
                this.followUpDate = value.Coerce();
            }
        }

        public DateTime DateAdded
        {
            get { return this.dateAdded; }
            protected set { this.dateAdded = value.Coerce(); }
        }

        public Guid ApplicationId { get; protected set; }

        public String ApplicationTitle { get; protected set; }

        public AccurateAppend.Accounting.LeadStatus Status { get; protected set; }

        public LeadSource LeadSource { get; protected set; }

        public LeadQualification Qualified { get; protected set; }

        public LeadContactMethod ContactMethod { get; protected set; }

        public String Website { get; protected set; }

        public LeadScore Score { get; protected set; }

        public String Address { get; protected set; }

        public String City { get; protected set; }

        public String State { get; protected set; }

        public String Zip { get; protected set; }

        public Guid OwnerId { get; protected set; }

        public String OwnerUserName { get; protected set; }

        #endregion

        #region Computed Values

        public String LeadStatusDescription => this.Status.GetDescription();

        public Int32 AgeInDays => (DateTime.UtcNow - this.DateAdded).Days;
        
        public String QualifiedDescription => this.Qualified.GetDescription();

        public String CompositeName
        {
            get
            {
                var value = PartyExtensions.BuildCompositeName(this.FirstName, this.LastName, this.BusinessName);
                if (!String.IsNullOrEmpty(value)) return value;

                return !String.IsNullOrEmpty(this.Website) ? this.Website : "No name";
            }
        }

        public String LastUpdateDescription
        {
            get
            {
                var ts = DateTime.UtcNow.Subtract(this.LastUpdate);
                return ts.ToReadableString();
            }
        }

        #endregion
    }

    internal class LeadViewConfiguration : EntityTypeConfiguration<LeadView>
    {
        public LeadViewConfiguration()
        {
            this.ToTable("LeadsView", "admin");

            // Primary Key
            this.HasKey(l => l.LeadId);

            // Ignore derived properties
            this.Ignore(l => l.CompositeName);
            this.Ignore(l => l.LeadStatusDescription);
            this.Ignore(l => l.AgeInDays);
            this.Ignore(l => l.QualifiedDescription);
            this.Ignore(l => l.LastUpdateDescription);

            this.Property(l => l.ApplicationId);
            this.Property(l => l.ApplicationTitle).HasColumnName("ApplicationName").IsUnicode(false).HasMaxLength(50);
            this.Property(l => l.BusinessName).HasColumnName("BusinessName").IsUnicode(false).HasMaxLength(100);
            this.Property(l => l.ContactMethod);
            this.Property(l => l.DateAdded);
            this.Property(l => l.Email).IsUnicode(false).HasMaxLength(250);
            this.Property(l => l.FirstName).IsUnicode(false).HasMaxLength(100);
            this.Property(l => l.FollowUpDate);
            this.Property(l => l.LastName).IsUnicode(false).HasMaxLength(100);
            this.Property(l => l.LastUpdate);
            this.Property(l => l.LeadSource);
            this.Property(l => l.NoteCount);
            this.Property(l => l.Phone).IsUnicode(false).HasMaxLength(20);
            this.Property(l => l.Qualified);
            this.Property(l => l.Status);
            this.Property(l => l.Website).IsUnicode(false).HasMaxLength(250);
            this.Property(l => l.Score);
            this.Property(l => l.DoNotMarketTo);

            this.Property(l => l.Address).IsUnicode(false).HasMaxLength(100);
            this.Property(l => l.City).IsUnicode(false).HasMaxLength(50);
            this.Property(l => l.State).IsUnicode(false).HasMaxLength(50);
            this.Property(l => l.Zip).IsUnicode(false).HasMaxLength(50);

            this.Property(l => l.OwnerId);
            this.Property(l => l.OwnerUserName).IsUnicode().HasMaxLength(256);
        }
    }
}
