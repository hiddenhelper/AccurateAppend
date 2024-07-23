using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="ProductCategory"/> entity type.
    /// </summary>
    public class ProductCategoryConfiguration : EntityTypeConfiguration<ProductCategory>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategoryConfiguration"/> class.
        /// </summary>
        public ProductCategoryConfiguration()
        {
            // Map to Table
            this.ToTable("Category", "sales");

            // Primary Key
            this.HasKey(p => p.Id);

            // Map Properties
            this.Property(p => p.Id).HasColumnName("CategoryId");

            this.Property(p => p.Name).HasColumnName("Key").IsUnicode(false).HasMaxLength(100).IsRequired();
            this.Property(p => p.Description).HasColumnName("Title").IsUnicode(false).HasMaxLength(100).IsRequired();
        }
    }
}
