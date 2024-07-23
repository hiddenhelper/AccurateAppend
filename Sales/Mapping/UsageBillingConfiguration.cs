using System;
using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="UsageBilling"/> entity type.
    /// </summary>
    public class UsageBillingConfiguration : EntityTypeConfiguration<UsageBilling>
    {
        /// <summary>
        /// Holds the value (3) in the discriminator column for the <see cref="UsageBilling"/> entity.
        /// </summary>
        public const Int32 Type = 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsageBillingConfiguration"/> class.
        /// </summary>
        public UsageBillingConfiguration()
        {
            // Discriminator
            this.Map<UsageBilling>(m => m.Requires("Type").HasValue(Type));

            // Map Properties
            this.Property(s => s.Recurrence).HasColumnName("Recurrence").IsRequired();
            this.Property(a => a.MaxBalance).HasColumnName("Limit").IsOptional().HasPrecision(19, 4);
        }
    }
}
