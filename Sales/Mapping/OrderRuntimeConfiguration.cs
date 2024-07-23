using AccurateAppend.Data;
using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="OrderRuntime"/> entity type.
    /// </summary>
    public class OrderRuntimeConfiguration : EntityTypeConfiguration<OrderRuntime>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderRuntimeConfiguration"/> class.
        /// </summary>
        public OrderRuntimeConfiguration()
        {
            // Map to Table
            this.ToTable("Orders", "sales");

            // Primary Key
            this.HasKey(m => m.Id);

            // derived properties
            this.Ignore(j => j.Report);

            // Map Properties
            this.Property("ReportInternal").HasColumnName("Report").IsRequired();
            this.Property(r => r.Id).HasColumnName("OrderId").IsRequired();

            // Map associations
            this.HasRequired(r => r.Source).WithRequiredDependent(d => d.Processing);
        }
    }
}
