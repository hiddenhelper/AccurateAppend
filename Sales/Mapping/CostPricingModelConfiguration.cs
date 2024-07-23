using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="CostPricingModel"/> entity type.
    /// </summary>
    public class CostPricingModelConfiguration : EntityTypeConfiguration<CostPricingModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CostPricingModelConfiguration"/> class.
        /// </summary>
        public CostPricingModelConfiguration()
        {
            // Map to Table
            this.ToTable("CustomRateCardPricingModels", "sales");

            // Primary Key
            this.HasKey(m => new { m.Category, m.ProductId });

            // Map Properties
            this.Property(m => m.Category).IsUnicode(false).HasMaxLength(50).IsRequired();
            this.Property(m => m.Model).IsRequired();
            this.Property(m => m.ProductId).IsRequired();

            // Map associations
            this.HasRequired(m => m.ForProduct).WithMany();
        }
    }
}
