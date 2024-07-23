using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Websites.Clients.Data
{
    /// <summary>
    /// Mapping configuration for the <see cref="BatchOrder"/> entity.
    /// </summary>
    public class BatchOrderConfiguration : EntityTypeConfiguration<BatchOrder>
    {
        public BatchOrderConfiguration()
        {
            this.ToTable("BatchOrder", "clients");

            this.Ignore(o => o.SourceDescription);

            this.Property(o => o.JobStatus).IsRequired();
        }
    }
}