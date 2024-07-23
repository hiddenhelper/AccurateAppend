using System;
using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="UsageBilling"/> entity type.
    /// </summary>
    public class AccrualBillingConfiguration : EntityTypeConfiguration<AccrualBilling>
    {
        /// <summary>
        /// Holds the value (2) in the discriminator column for the <see cref="AccrualBilling"/> entity.
        /// </summary>
        public const Int32 Type = 2;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccrualBillingConfiguration"/> class.
        /// </summary>
        public AccrualBillingConfiguration()
        {
            // Discriminator
            this.Map<AccrualBilling>(m => m.Requires("Type").HasValue(Type));

            // Map Properties
            this.Property(a => a.MaxAccrualAmount).HasColumnName("Limit").IsRequired().HasPrecision(19, 4);
        }
    }
}
