using System.Data.Entity.ModelConfiguration;

namespace AccurateAppend.Websites.Clients.Data
{
    /// <summary>
    /// Mapping configuration for the <see cref="Order"/> entity.
    /// </summary>
    public class OrderConfiguration : EntityTypeConfiguration<Order>
    {
        public OrderConfiguration()
        {
            this.ToTable("Orders", "clients");

            // Primary Key
            this.HasKey(o => o.Id);

            this.Ignore(o => o.Type);
            this.Ignore(o => o.StatusDescription);
            this.Ignore(o => o.CanDownload);
            this.Ignore(o => o.DownloadLink);
            this.Ignore(o => o.ReceiptDownloadLink);

            this.Property(o => o.Id).HasColumnName("OrderId").IsRequired();
            this.Property(o => o.CustomerFileName).IsOptional().IsUnicode(false).HasMaxLength(250);
            this.Property(o => o.DateSubmitted).IsRequired();
            this.Property(o => o.RequestId).IsRequired();
            this.Property(o => o.Name).IsRequired();
            this.Property(o => o.SystemFileName).IsOptional().IsUnicode(false).HasMaxLength(250);
            this.Property(o => o.TotalRecords).IsRequired();
            this.Property(o => o.UserId).IsRequired();
            this.Property(o => o.OrderStatus).IsRequired();
            this.Property(o => o.BillId).IsOptional();
        }
    }
}