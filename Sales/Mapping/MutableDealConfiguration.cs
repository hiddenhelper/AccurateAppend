using System;
using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// /// Performs the default EF mapping for the <see cref="MutableDeal"/> entity type.
    /// </summary>
    public class MutableDealConfiguration : EntityTypeConfiguration<MutableDeal>
    {
        /// <summary>
        /// Holds the value (0) in the discriminator column for the <see cref="MutableDeal"/> entity.
        /// </summary>
        public const Int32 DealType = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MutableDealConfiguration"/> class.
        /// </summary>
        public MutableDealConfiguration()
        {
            this.Map<MutableDeal>(m => m.Requires("DealType").HasValue(DealType));
        }
    }
}