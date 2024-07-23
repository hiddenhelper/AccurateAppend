using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="Order"/> entity type.
    /// </summary>
    public class OrderConfiguration : EntityTypeConfiguration<Order>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderConfiguration"/> class.
        /// </summary>
        public OrderConfiguration()
        {
            // Map to Table
            this.ToTable("Orders", "sales");

            // Primary Key
            this.HasKey(o => o.Id);

            // Map Properties
            this.Property(o => o.Id).HasColumnName("OrderId").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(o => o.OrderMinimum).HasColumnName("CostAdjustment").HasPrecision(19, 4).IsOptional();
            this.Property(o => o.CompletedDate).HasColumnName("DateComplete").IsRequired();
            this.Property(o => o.CreatedDate).HasColumnName("DateAdded").IsRequired();
            this.Property(o => o.PublicKey).IsUnicode(false).HasMaxLength(50).IsRequired();
            this.Property(o => o.Status).IsRequired();

            // Map associations
            this.HasMany(o => o.Lines).WithRequired(i => i.Order).Map(m => m.MapKey("OrderId")).WillCascadeOnDelete();
            this.HasRequired(o => o.Bill).WithRequiredPrincipal(b => b.Order);
            this.HasOptional(o => o.Content).WithRequired(m => m.Order).Map(m => m.MapKey("OrderId"));
            this.HasMany(o => o.Transactions).WithRequired(e => e.Order).Map(m => m.MapKey("OrderId"));
            this.HasMany(o => o.PendingTransactions).WithRequired(e => e.Order).Map(m => m.MapKey("OrderId"));
            this.HasRequired(o => o.Processing).WithRequiredPrincipal(d => d.Source);
        }
    }
}
