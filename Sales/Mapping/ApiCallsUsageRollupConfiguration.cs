using System;
using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping extension for the <see cref="ApiCallsUsageRollup"/> entity type.
    /// </summary>
    public class ApiCallsUsageRollupConfiguration : EntityTypeConfiguration<ApiCallsUsageRollup>
    {
        /// <summary>
        /// Holds the value (2) in the discriminator column for the <see cref="ApiCallsUsageRollup"/> entity.
        /// </summary>
        public const Int32 Source = 2;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiCallsUsageRollupConfiguration"/> class.
        /// </summary>
        public ApiCallsUsageRollupConfiguration()
        {
            this.Map<ApiCallsUsageRollup>(m => m.Requires("Source").HasValue(Source));
        }
    }
}
