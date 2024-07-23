using System.Data.Entity.ModelConfiguration;
using AccurateAppend.Data;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="CsvCart"/> entity type.
    /// </summary>
    public class CsvCartConfiguration : EntityTypeConfiguration<CsvCart>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvCartConfiguration"/> class.
        /// </summary>
        public CsvCartConfiguration()
        {
            this.ToTable("CsvCart", "sales");

            // Primary Key
            this.HasKey(c => c.Id);

            // derived properties
            this.Ignore(c => c.Analysis);
            this.Ignore(c => c.Manifest);

            // map properties
            this.Property(c => c.DateCreated);
            this.Property("AnalysisInternal").HasColumnName("Analysis").IsOptional();
            this.Property(c => c.SystemFileName).IsUnicode(false).HasMaxLength(40);
            this.Property(c => c.ManifestId).IsOptional();
            this.Property("ManifestInternal").HasColumnName("Manifest").IsOptional();
        }
    }
}