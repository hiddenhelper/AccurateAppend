using System;
using System.Data.Entity.ModelConfiguration;
using AccurateAppend.Core;

namespace DomainModel.ReadModel
{
    public class NationBuilderRegistration
    {
        #region Fields

        private DateTime dateRegistered;

        #endregion

        #region Constructor

        /// <summary>
        /// This type is readonly.
        /// </summary>
        protected NationBuilderRegistration()
        {
        }

        #endregion

        #region Properties

        public Int32 Id { get; protected set; }

        public String UserName { get; protected set; }

        public Guid UserId { get; protected set; }

        public Guid ApplicationId { get; protected set; }

        public String Slug { get; protected set; }

        public DateTime DateRegistered
        {
            get { return this.dateRegistered; }
            protected set { this.dateRegistered = value.Coerce(); }
        }

        public Int32 ReportOrders { get; protected set; }

        public Int32 AppendOrders { get; protected set; }

        public Int32? RecordCount { get; protected set; }

        public String AccessToken { get; protected set; }

        #endregion
    }

    internal class NationBuilderRegistrationConfiguration : EntityTypeConfiguration<NationBuilderRegistration>
    {
        public NationBuilderRegistrationConfiguration()
        {
            this.ToTable("NationBuilderRegistrations", "admin");

            this.HasKey(r => r.Id);

            this.Property(r => r.Id);
            this.Property(r => r.DateRegistered);
            this.Property(r => r.AppendOrders).HasColumnName("PushCount");
            this.Property(r => r.ReportOrders).HasColumnName("ReportCount");
            this.Property(r => r.ApplicationId);
            this.Property(r => r.RecordCount).HasColumnName("PersonCount");
            this.Property(r => r.Slug).IsUnicode(false).HasMaxLength(50);
            this.Property(r => r.UserId);
            this.Property(r => r.UserName).IsUnicode(true).HasMaxLength(256);
            this.Property(r => r.AccessToken).HasColumnName("LatestAccessToken").IsUnicode(false).HasMaxLength(64);
        }
    }
}
