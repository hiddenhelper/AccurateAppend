using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="Cost"/> entity type.
    /// </summary>
    public class CostConfiguration : EntityTypeConfiguration<Cost>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CostConfiguration"/> class.
        /// </summary>
        public CostConfiguration()
        {
            // Map to Table
            this.ToTable("Cost", "sales");

            // Primary Key
            this.HasKey(c => c.Id);

            // Map Properties
            this.Property(c => c.Id).HasColumnName("CostId");

            this.Property(c => c.Ceiling).HasColumnName("UpperRangeCost").IsRequired();
            this.Property(c => c.Floor).HasColumnName("LowRangeCost").IsRequired();
            this.Property(c => c.PerMatch).HasColumnName("MatchCost").IsRequired().HasPrecision(19, 4);
            this.Property(c => c.PerRecord).HasColumnName("RecordCost").IsRequired().HasPrecision(19, 4);
            this.Property(c => c.Category).IsUnicode(false).HasMaxLength(50).IsRequired();

            // Map associations
            this.HasRequired(c => c.Product).WithMany().Map(m => m.MapKey("ProductId"));
        }
    }
}
