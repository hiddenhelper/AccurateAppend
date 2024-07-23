using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="ProductOrder"/> entity type.
    /// </summary>
    public class ProductOrderConfiguration : EntityTypeConfiguration<ProductOrder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductOrderConfiguration"/> class.
        /// </summary>
        public ProductOrderConfiguration()
        {
            this.ToTable("ProductOrder", "sales");

            // Primary Key
            this.HasKey(c => c.Id);

            // map properties
            this.Property(o => o.Id).HasColumnName("OrderId");
            this.Property(o => o.DateSubmitted).IsRequired();
            this.Property(o => o.Source);
            this.Property(o => o.Status).IsRequired();
            this.Property(o => o.Name).HasColumnName("ListName").IsUnicode().HasMaxLength(255);
            this.Property(o => o.RecordCount).HasColumnName("TotalRecords").IsRequired();

            // Relationships
            this.HasRequired(c => c.Client).WithMany().Map(m => m.MapKey("UserId"));
        }
    }
}
