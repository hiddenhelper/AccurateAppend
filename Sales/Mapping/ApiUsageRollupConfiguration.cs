using System;
using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping extension for the <see cref="ApiUsageRollup"/> entity type.
    /// </summary>
    public class ApiUsageRollupConfiguration : EntityTypeConfiguration<ApiUsageRollup>
    {
        /// <summary>
        /// Holds the value (1) in the discriminator column for the <see cref="ApiUsageRollup"/> entity.
        /// </summary>
        public const Int32 Source = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiUsageRollupConfiguration"/> class.
        /// </summary>
        public ApiUsageRollupConfiguration()
        {
            this.Map<ApiUsageRollup>(m => m.Requires("Source").HasValue(Source));
        }
    }
}
