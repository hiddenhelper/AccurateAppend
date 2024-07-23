using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Websites.Clients.Data
{
    /// <summary>
    /// Mapping configuration for the <see cref="DirectClientOrder"/> entity.
    /// </summary>
    public class DirectClientOrderConfiguration : EntityTypeConfiguration<DirectClientOrder>
    {
        public DirectClientOrderConfiguration()
        {
            this.ToTable("DirectClientOrder", "clients");

            this.Ignore(o => o.SourceDescription);

            this.Property(o => o.JobStatus).IsRequired();
        }
    }
}