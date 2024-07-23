using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="CreditCardRef"/> entity type.
    /// </summary>
    public class CreditCardRefConfiguration : EntityTypeConfiguration<CreditCardRef>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CreditCardRefConfiguration"/> class.
        /// </summary>
        public CreditCardRefConfiguration()
        {
            // Map to Table
            this.ToTable("CreditCardReference", "sales");

            // Primary Key
            this.HasKey(c => c.Id);

            // Map Properties
            this.Property(c => c.Id).HasColumnName("UserCreditCardId");

            this.Property(c => c.IsPrimary).IsRequired();
            this.Property(c => c.CardExpiration).HasColumnName("CardExp").IsUnicode(false).IsFixedLength().HasMaxLength(4).IsRequired();
            this.Property(c => c.DisplayValue).HasMaxLength(8).IsRequired();
            this.Property(c => c.PublicKey).IsRequired();

            // Map associations
            this.HasRequired(c => c.Client).WithMany().Map(m => m.MapKey("UserId"));
        }
    }
}
