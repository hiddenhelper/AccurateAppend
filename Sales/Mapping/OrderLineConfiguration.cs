using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="ProductLine"/> entity type.
    /// </summary>
    public class ProductLineConfiguration : EntityTypeConfiguration<ProductLine>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductLineConfiguration"/> class.
        /// </summary>
        public ProductLineConfiguration()
        {
            // Map to Table
            this.ToTable("OrderItem", "sales");

            // Primary Key
            this.HasKey(l => l.Id);

            // Map Properties
            this.Property(l => l.Id).HasColumnName("OrderItemId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(l => l.Description).IsUnicode(false).HasMaxLength(250).IsRequired();
            this.Property(l => l.Quantity).IsRequired();
            this.Property(l => l.Price).IsRequired().HasPrecision(19, 4);
            this.Property(l => l.Maximum).IsOptional();

            // Map associations
            this.HasRequired(l => l.Product).WithMany().Map(m => m.MapKey("ProductId"));
            this.HasRequired(l => l.Order);
        }
    }
}
