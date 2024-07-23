using System;
using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="SubscriptionBilling"/> entity type.
    /// </summary>
    public class SubscriptionBillingConfiguration : EntityTypeConfiguration<SubscriptionBilling>
    {
        /// <summary>
        /// Holds the value (1) in the discriminator column for the <see cref="SubscriptionBilling"/> entity.
        /// </summary>
        public const Int32 Type = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionBillingConfiguration"/> class.
        /// </summary>
        public SubscriptionBillingConfiguration()
        {
            // Discriminator
            this.Map<SubscriptionBilling>(m => m.Requires("Type").HasValue(Type));

            // Map Properties
            this.Property(s => s.PrepaymentAmount).HasColumnName("Amount").IsOptional();
            this.Property(s => s.Recurrence).HasColumnName("Recurrence").IsRequired();
            this.Property(a => a.MaxOverageLimit).HasColumnName("Limit").IsOptional().HasPrecision(19, 4);
            this.Property(s => s.FixedRate).IsRequired();
        }
    }
}
