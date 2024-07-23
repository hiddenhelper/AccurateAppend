using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Websites.Clients.Data
{
    /// <summary>
    /// Mapping configuration for the <see cref="NationBuilderOrder"/> entity.
    /// </summary>
    public class NationBuilderOrderConfiguration : EntityTypeConfiguration<NationBuilderOrder>
    {
        public NationBuilderOrderConfiguration()
        {
            this.ToTable("NationBuilderOrder", "clients");

            this.Ignore(o => o.Source);

            this.Property(o => o.Slug).IsRequired();
            this.Property(o => o.PushStatus).IsRequired();
            this.Property(o => o.CurrentPage).IsRequired();
        }
    }
}