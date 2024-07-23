using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="PostalAddressRef"/> entity type.
    /// </summary>
    public class PostalAddressRefConfiguration : EntityTypeConfiguration<PostalAddressRef>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostalAddressRefConfiguration"/> class.
        /// </summary>
        public PostalAddressRefConfiguration()
        {
            // To table
            this.Map(m =>
            {
                m.ToTable("ClientReference", "sales");
            });

            // Primary Key
            this.HasKey(n => n.Id);

            this.Property(a => a.Id).HasColumnName("UserId").IsRequired();
            this.Property(a => a.StreetAddress).HasColumnName("Address").IsOptional().IsUnicode().HasMaxLength(100);
            this.Property(a => a.City).HasColumnName("City").IsOptional().IsUnicode().HasMaxLength(50);
            this.Property(a => a.State).HasColumnName("State").IsOptional().IsUnicode().HasMaxLength(50);
            this.Property(a => a.PostalCode).HasColumnName("Zip").IsOptional().IsUnicode().HasMaxLength(50);
            this.Property(a => a.Country).HasColumnName("Country").IsOptional().IsUnicode(false).HasMaxLength(50);

            // Map associations
            this.HasRequired(a => a.Client).WithRequiredDependent(c => c.Address);
        }
    }
}
