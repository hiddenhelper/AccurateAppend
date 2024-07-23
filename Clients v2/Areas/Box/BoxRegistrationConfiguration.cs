using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Websites.Clients.Areas.Box
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="BoxRegistration"/> entity type.
    /// </summary>
    public class BoxRegistrationConfiguration : EntityTypeConfiguration<BoxRegistration>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoxRegistrationConfiguration"/> class.
        /// </summary>
        public BoxRegistrationConfiguration()
        {
            // Map to Table
            this.ToTable("BoxRegistration", "integration");

            // Primary Key
            this.HasKey(s => s.Id);

            // Map Properties
            this.Property(m => m.Id).HasColumnName("BoxRegistrationId").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(m => m.Name).IsUnicode().HasMaxLength(50).IsRequired();
            this.Property(m => m.UserId).IsRequired();
            this.Property(m => m.DateRegistered).IsRequired();
            this.Property(m => m.UserId).IsRequired();
            this.Property(m => m.AccessToken).IsUnicode(false).HasMaxLength(32).IsRequired();
            this.Property(m => m.RefreshToken).IsUnicode(false).HasMaxLength(64).IsRequired();
            this.Property(m => m.DateExpires).IsRequired();
            this.Property(m => m.DateGranted).IsRequired();
            this.Property(m => m.PublicKey).IsRequired();
        }
    }
}