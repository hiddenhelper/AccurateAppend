using System;
using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="BillableOrder"/> entity type.
    /// </summary>
    public class BillableOrderConfiguration : EntityTypeConfiguration<BillableOrder>
    {
        /// <summary>
        /// Holds the value (0) in the discriminator column for the <see cref="BillableOrder"/> entity.
        /// </summary>
        public const Int32 OrderType = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="BillableOrderConfiguration"/> class.
        /// </summary>
        public BillableOrderConfiguration()
        {
            this.Map<BillableOrder>(m => m.Requires("OrderType").HasValue(OrderType));

            this.Property(d => d.PerformAutoBilling).HasColumnName("EnableAutoBill").IsOptional();
        }
    }
}
