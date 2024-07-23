using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="LedgerEntry"/> entity type.
    /// </summary>
    public class LedgerEntryConfiguration : EntityTypeConfiguration<LedgerEntry>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LedgerEntryConfiguration"/> class.
        /// </summary>
        public LedgerEntryConfiguration()
        {
            // Map to Table
            this.ToTable("AccountLedger", "sales");

            // Primary Key
            this.HasKey(s => s.Id);

            // Map Properties
            this.Property(a => a.Id).HasColumnName("Id").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(a => a.PeriodStart).IsRequired();
            this.Property(a => a.PeriodEnd).IsRequired();
            this.Property(a => a.Classification).HasColumnName("EntryType").IsRequired();

            // Map associations
            this.HasRequired(a => a.ForAccount).WithMany().Map(m => m.MapKey("AccountId"));
            this.HasOptional(a => a.WithDeal).WithOptionalDependent().Map(m => m.MapKey("DealId"));
        }
    }
}
