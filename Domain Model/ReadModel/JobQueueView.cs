using System;
using System.Data.Entity.ModelConfiguration;
using AccurateAppend.Core;
using AccurateAppend.Core.Definitions;
using AccurateAppend.JobProcessing;
using DomainModel.Enum;

namespace DomainModel.ReadModel
{
    /// <summary>
    /// A job data view model.
    /// </summary>
    public class JobQueueView
    {
        private DateTime dateComplete;
        private DateTime dateUpdated;
        private DateTime dateSubmitted;

        #region Constructor

        /// <summary>
        /// This is a readonly type.
        /// </summary>
        protected JobQueueView()
        {
        }

        #endregion

        #region Properties

        public Int32 JobId { get; protected set; }

        public Guid UserId { get; protected set; }

        public Guid ApplicationId { get; protected set; }

        public DateTime DateComplete
        {
            get { return this.dateComplete; }
            protected set { this.dateComplete = value.ToSafeLocal(); }
        }

        public DateTime DateUpdated
        {
            get { return this.dateUpdated; }
            protected set { this.dateUpdated = value.ToSafeLocal(); }
        }

        public DateTime DateSubmitted
        {
            get { return this.dateSubmitted; }
            protected set { this.dateSubmitted = value.ToSafeLocal(); }
        }

        public Int32 MatchCount { get; protected set; }

        public Int32 RecordCount { get; protected set; }

        public Int32 ProcessedCount { get; protected set; }
        
        public String UserName { get; protected set; }

        public String CustomerFileName { get; protected set; }

        public String InputFileName { get; protected set; }

        public JobStatus Status { get; protected set; }

        public String Product { get; protected set; }

        public JobSource Source { get; protected set; }

        public Decimal MatchRate => this.MatchCount == 0 || this.ProcessedCount == 0 ? 0 : (Decimal) this.MatchCount / this.ProcessedCount;

        public Priority Priority { get; protected set; }

        public Boolean IsPaused { get; protected set; }

        public String EstimatedCompletionTime
        {
            get
            {
                if (this.Status.GetCategoryDescription() == JobStatusExtensions.Category.PostProcessing) return null;

                // calculate estimatedCompletion
                if (this.Status != JobStatus.InProcess) return null;

                if (this.RecordCount == 0 || this.ProcessingRate == 0) return null;

                var recordsRemaining = this.RecordCount - this.ProcessedCount;
                var timeRemaining = Convert.ToDouble(recordsRemaining) / Convert.ToDouble(this.ProcessingRate);
                DateTime? estimatedCompletionTime = DateTime.Now.AddMinutes(timeRemaining < 1 ? 1 : timeRemaining);

                return estimatedCompletionTime.ToString();
            }
        }

        protected internal Int32 ProcessingRate { get; protected set; }

        #endregion
    }

    internal class JobQueueViewConfiguration : EntityTypeConfiguration<JobQueueView>
    {
        public JobQueueViewConfiguration()
        {
            this.ToTable("JobQueueView", "admin");

            // Primary Key
            this.HasKey(c => c.JobId);

            // Ignore derived properties
            this.Ignore(c => c.MatchRate);
            this.Ignore(c => c.EstimatedCompletionTime);

            this.Property(c => c.ApplicationId);
            this.Property(c => c.CustomerFileName).IsUnicode(false).HasMaxLength(250);
            this.Property(c => c.InputFileName).IsUnicode(false).HasMaxLength(250);
            this.Property(c => c.DateComplete);
            this.Property(c => c.DateUpdated);
            this.Property(c => c.ProcessedCount);
            this.Property(c => c.DateSubmitted);
            this.Property(c => c.JobId);
            this.Property(c => c.MatchCount);
            this.Property(c => c.Product).IsUnicode(false).HasMaxLength(255);
            this.Property(c => c.RecordCount);
            this.Property(c => c.Source);
            this.Property(c => c.Status);
            this.Property(c => c.UserName).IsUnicode(true).HasMaxLength(256);
            this.Property(j => j.Priority);
            this.Property(j => j.ProcessingRate);
            this.Property(j => j.IsPaused);
        }
    }
}
