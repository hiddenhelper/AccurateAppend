using System;
using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping extension for the <see cref="BatchUsageRollup"/> entity type.
    /// </summary>
    public class BatchUsageRollupConfiguration : EntityTypeConfiguration<BatchUsageRollup>
    {
        /// <summary>
        /// Holds the value (0) in the discriminator column for the <see cref="BatchUsageRollup"/> entity.
        /// </summary>
        public const Int32 Source = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchUsageRollupConfiguration"/> class.
        /// </summary>
        public BatchUsageRollupConfiguration()
        {
            this.Map<BatchUsageRollup>(m => m.Requires("Source").HasValue(Source));
        }
    }
}
