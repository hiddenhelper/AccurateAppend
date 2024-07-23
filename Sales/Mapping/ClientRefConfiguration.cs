using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="ClientRef"/> entity type.
    /// </summary>
    public class ClientRefConfiguration : EntityTypeConfiguration<ClientRef>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientRefConfiguration"/> class.
        /// </summary>
        public ClientRefConfiguration()
        {
            // To table
            this.Map(m =>
            {
                m.ToTable("ClientReference", "sales");
            });

            // Primary Key
            this.HasKey(c => c.UserId);

            // Map Properties

            this.Property(c => c.UserId).IsRequired();
            this.Property(c => c.ApplicationId).IsRequired();
            this.Property(c => c.BusinessName).HasColumnName("BusinessName").IsUnicode(false).HasMaxLength(100);
            this.Property(c => c.FirstName).HasColumnName("FirstName").IsUnicode(false).HasMaxLength(100);
            this.Property(c => c.LastName).HasColumnName("LastName").IsUnicode(false).HasMaxLength(100);
            this.Property(c => c.UserName).HasColumnName("Email").IsUnicode(false).HasMaxLength(250).IsRequired();
            this.Property(c => c.OwnerId).IsRequired();
            this.Property(c => c.PrimaryPhone).HasColumnName("Phone").IsOptional().IsUnicode(false).HasMaxLength(20);

            // Map associations
            this.HasRequired(c => c.Address).WithRequiredPrincipal(a => a.Client);
        }
    }
}