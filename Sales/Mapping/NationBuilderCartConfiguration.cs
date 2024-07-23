using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="NationBuilderCart"/> entity type.
    /// </summary>
    public class NationBuilderCartConfiguration : EntityTypeConfiguration<NationBuilderCart>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CartConfiguration"/> class.
        /// </summary>
        public NationBuilderCartConfiguration()
        {
            this.ToTable("NationBuilderCart", "sales");

            // Primary Key
            this.HasKey(c => c.Id);

            // map properties
            this.Property(c => c.ExternalId).IsOptional();
            this.Property(c => c.IntegrationId).IsOptional();
        }
    }
}