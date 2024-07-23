using System.Data.Entity.ModelConfiguration;
using AccurateAppend.Data;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="Cart"/> entity type.
    /// </summary>
    public class CartConfiguration : EntityTypeConfiguration<Cart>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CartConfiguration"/> class.
        /// </summary>
        public CartConfiguration()
        {
            this.ToTable("Cart", "sales");

            // Primary Key
            this.HasKey(c => c.Id);

            // derived properties
            this.Ignore(c => c.Quote);

            // map properties
            this.Property(c => c.DateCreated);
            this.Property(c => c.Id).HasColumnName("CartId");
            this.Property(c => c.IsActive);
            this.Property(c => c.Name).IsUnicode().HasMaxLength(255);
            this.Property("QuoteInternal").HasColumnName("QuotedEstimate").IsOptional();

            // Relationships
            this.HasRequired(c => c.Client).WithMany().Map(m => m.MapKey("UserId"));
        }
    }
}
