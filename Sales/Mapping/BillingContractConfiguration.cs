
using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="BillingContract"/> entity type.
    /// </summary>
    public class BillingContractConfiguration : EntityTypeConfiguration<BillingContract>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BillingContractConfiguration"/> class.
        /// </summary>
        public BillingContractConfiguration()
        {
            // To table
            this.ToTable("Orders", "sales");

            // Primary Key
            this.HasKey(b => b.OrderId);

            this.HasRequired(b => b.Order).WithRequiredDependent(o => o.Bill);
            this.Property(b => b.ContractType).HasColumnName("BillType").IsOptional();
        }
    }
}
