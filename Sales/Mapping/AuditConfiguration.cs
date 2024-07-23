using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="Audit"/> entity type.
    /// </summary>
    public class AuditConfiguration : EntityTypeConfiguration<Audit>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuditConfiguration"/> class.
        /// </summary>
        public AuditConfiguration()
        {
            // Map to Table
            this.ToTable("Notes", "operations");

            // Primary Key
            this.HasKey(a => a.Id);

            // Map Properties
            this.Property(a => a.Id).HasColumnName("NoteId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(a => a.CreatedDate).HasColumnName("DateAdded").IsRequired();
            this.Property(a => a.Content).HasColumnName("Body").HasMaxLength(4000).IsRequired();
            this.Property(a => a.CreatedBy).HasColumnName("AddedBy").IsRequired();
        }
    }
}
