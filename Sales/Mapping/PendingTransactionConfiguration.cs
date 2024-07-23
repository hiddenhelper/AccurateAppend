using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Sales.Mapping
{
    /// <summary>
    /// Performs the default EF mapping for the <see cref="PendingTransaction"/> entity type.
    /// </summary>
    public class PendingTransactionConfiguration : EntityTypeConfiguration<PendingTransaction>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionEventConfiguration"/> class.
        /// </summary>
        public PendingTransactionConfiguration()
        {
            this.ToTable("PendingTransactions", "sales");

            // Primary Key
            this.HasKey(c => c.Id);
            
            // Map Properties
            this.Property(c => c.Id).HasColumnName("TransactionId").IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(c => c.PublicKey).IsRequired();
            this.Property(c => c.AmountRequested).HasColumnName("Requested").HasPrecision(16, 2).IsRequired();
            this.Property(c => c.DisplayValue).IsUnicode(false).IsFixedLength().HasMaxLength(4).IsRequired();
        }
    }
}