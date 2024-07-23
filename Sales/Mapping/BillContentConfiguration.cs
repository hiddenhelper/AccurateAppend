using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using AccurateAppend.Data;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="BillContent"/> entity type.
    /// </summary>
    public class BillContentConfiguration : EntityTypeConfiguration<BillContent>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BillContentConfiguration"/> class.
        /// </summary>
        public BillContentConfiguration()
        {
            // Map to Table
            this.ToTable("BillContent", "sales");

            // Primary Key
            this.HasKey(s => s.Id);

            // Map Properties
            this.Property(m => m.Id).HasColumnName("BillContentId").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(m => m.Body).HasColumnName("MessageBody").IsRequired().HasMaxLength(null);
            this.Property(m => m.IsHtml).IsOptional();
            this.Property(m => m.SendFrom).IsRequired().IsUnicode(false).HasMaxLength(254);
            this.Property(m => m.Subject).HasColumnName("MessageSubject").IsRequired().HasMaxLength(255);

            // Map Private Properties
            this.Property("AttachmentsDb").HasColumnName("Attachments");
            this.Property("BccToDb").HasColumnName("BccTo");
            this.Property("SendToDb").HasColumnName("SendTo");

            // Ignore Projected Properties
            this.Ignore(m => m.Attachments);
            this.Ignore(m => m.BccTo);
            this.Ignore(m => m.SendTo);
        }
    }
}
