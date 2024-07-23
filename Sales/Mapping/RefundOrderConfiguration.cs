using System;
using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="RefundOrder"/> entity type.
    /// </summary>
    public class RefundOrderConfiguration : EntityTypeConfiguration<RefundOrder>
    {
        /// <summary>
        /// Holds the value (1) in the discriminator column for the <see cref="RefundOrder"/> entity.
        /// </summary>
        public const Int32 OrderType = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefundOrderConfiguration"/> class.
        /// </summary>
        public RefundOrderConfiguration()
        {
            this.Map<RefundOrder>(m => m.Requires("OrderType").HasValue(OrderType));
        }
    }
}