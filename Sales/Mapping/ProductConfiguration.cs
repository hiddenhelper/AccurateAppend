using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="Product"/> entity type.
    /// </summary>
    public class ProductConfiguration : EntityTypeConfiguration<Product>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductConfiguration"/> class.
        /// </summary>
        public ProductConfiguration()
        {
            // Map to Table
            this.ToTable("Product", "sales");

            // Primary Key
            this.HasKey(p => p.Id);

            // Map Properties
            this.Property(p => p.Id).HasColumnName("ProductId");

            this.Property(p => p.Description).IsUnicode(false).HasMaxLength(500).IsRequired();
            this.Property(p => p.Key).IsUnicode(false).HasMaxLength(100).IsRequired();
            this.Property(p => p.Title).IsUnicode(false).HasMaxLength(100).IsRequired();
            this.Property(p => p.Usage).HasColumnName("Status").IsRequired();
            this.Property(p => p.DefaultPricingModel).IsRequired();

            // Map associations
            this.HasRequired(p => p.Category).WithMany().Map(m => m.MapKey("CategoryId"));
        }
    }
}
