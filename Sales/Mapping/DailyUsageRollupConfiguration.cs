using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="DailyUsageRollup"/> entity type.
    /// </summary>
    public class DailyUsageRollupConfiguration : EntityTypeConfiguration<DailyUsageRollup>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DailyUsageRollupConfiguration"/> class.
        /// </summary>
        public DailyUsageRollupConfiguration()
        {
            // Map to Table
            this.ToTable("UsageRollup", "sales");

            // Primary Key
            this.HasKey(u => new { u.UserId, u.Date, u.Key });

            // Map Properties
            this.Property(u => u.UserId).IsRequired();
            this.Property(u => u.Date).HasColumnName("RollupDate").IsRequired();
            this.Property(u => u.Key).IsUnicode(false).HasMaxLength(50).IsRequired();

            this.Property(u => u.Count).IsRequired();
            this.Property(u => u.Matches).IsRequired();
        }
    }
}
