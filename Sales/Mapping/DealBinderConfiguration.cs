using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="DealBinder"/> entity type.
    /// </summary>
    public class DealBinderConfiguration : EntityTypeConfiguration<DealBinder>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DealBinderConfiguration"/> class.
        /// </summary>
        public DealBinderConfiguration()
        {
            // Map to Table
            this.ToTable("Deals", "sales");

            // Primary Key
            this.HasKey(d => d.Id);

            // Map Properties
            this.Property(d => d.Id).HasColumnName("DealId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(d => d.CreatedDate).HasColumnName("DateAdded").IsRequired();
            this.Property(d => d.Description).HasMaxLength(500).IsUnicode(false);
            this.Property(d => d.FollowupDate).IsRequired();
            this.Property(d => d.Status).IsRequired();
            this.Property(d => d.Title).HasMaxLength(150).IsUnicode(false).IsRequired();
            this.Property(d => d.Instructions).HasColumnName("ProcessingInstructions").IsMaxLength();
            this.Property(d => d.Amount).HasPrecision(19, 4).IsRequired();
            this.Property(d => d.OwnerId).HasColumnName("Creator_UserId");
            this.Property(d => d.SuppressNotifications).HasColumnName("SuppressNotifications");
            this.Property(d => d.CompletedDate).HasColumnName("DateComplete").IsOptional();

            // Map associations
            this.HasRequired(d => d.Client).WithMany().Map(m => m.MapKey("UserId"));
            this.HasMany(d => d.Orders).WithRequired(o => o.Deal).Map(m => m.MapKey("DealId")).WillCascadeOnDelete();
            this.HasMany(d => d.Notes).WithMany().Map(m =>
            {
                m.MapLeftKey("DealId");
                m.MapRightKey("NoteId");
                m.ToTable("DealNotes", "sales");
            });
        }
    }
}
