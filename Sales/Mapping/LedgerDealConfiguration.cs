using System;
using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="LedgerDeal"/> entity type.
    /// </summary>
    public class LedgerDealConfiguration : EntityTypeConfiguration<LedgerDeal>
    {
        /// <summary>
        /// Holds the value (1) in the discriminator column for the <see cref="LedgerDeal"/> entity.
        /// </summary>
        public const Int32 DealType = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="LedgerDealConfiguration"/> class.
        /// </summary>
        public LedgerDealConfiguration()
        {
            this.Map<LedgerDeal>(m => m.Requires("DealType").HasValue(DealType));
        }
    }
}