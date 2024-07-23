using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="RecurringBillingAccount"/> entity type.
    /// </summary>
    public class RecurringBillingAccountConfiguration : EntityTypeConfiguration<RecurringBillingAccount>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecurringBillingAccountConfiguration"/> class.
        /// </summary>
        public RecurringBillingAccountConfiguration()
        {
            // Map to Table
            this.ToTable("Subscriptions", "sales");

            // Primary Key
            this.HasKey(s => s.Id);

            // Map Properties
            this.Property(a => a.Id).HasColumnName("SubscriptionId").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(a => a.EffectiveDate).HasColumnName("StartDate").IsRequired();
            this.Property(a => a.EndDate).HasColumnName("EndDate").IsOptional();
            this.Property(a => a.SpecialProcessing).IsRequired();
            this.Property(a => a.PublicKey).IsRequired();

            // Map associations
            this.HasRequired(a => a.ForClient).WithMany().Map(m => m.MapKey("UserId"));
        }
    }
}
