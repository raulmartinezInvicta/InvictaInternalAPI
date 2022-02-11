using InvictaInternalAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvictaInternalAPI.Configurations
{
    public class EcommerceOrderConfiguration : IEntityTypeConfiguration<ECommerceOrder>
    {
        public void Configure(EntityTypeBuilder<ECommerceOrder> builder)
        {
            builder.ToTable("eCommerceOrder");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("ID");

            builder.Property(e => e.CompanyId).HasColumnName("CompanyID");

            builder.Property(e => e.CustomerEmail)
                .IsRequired()
                .HasMaxLength(250)
                .IsUnicode(false);

            builder.Property(e => e.CustomerId).HasColumnName("CustomerID");

            builder.Property(e => e.DbtimeStamp)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasColumnName("DBTimeStamp");

            builder.Property(e => e.DiscountAmount).HasColumnType("money");

            builder.Property(e => e.DiscountDesc)
                .IsRequired()
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            builder.Property(e => e.FedexTrackingNo)
                .HasMaxLength(100)
                .IsUnicode(false);

            builder.Property(e => e.IsPendingForwarding).HasColumnName("isPendingForwarding");

            builder.Property(e => e.OrderCurrencyCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValueSql("('USD')");

            builder.Property(e => e.OrderId).HasColumnName("OrderID");

            builder.Property(e => e.OrderNumber)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.PaymentAmountOrdered).HasColumnType("money");

            builder.Property(e => e.PaymentCctype)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PaymentCCType");

            builder.Property(e => e.SalesTax).HasColumnType("money");

            builder.Property(e => e.ShippingAddressType)
                .HasMaxLength(250)
                .IsUnicode(false);

            builder.Property(e => e.ShippingAmount).HasColumnType("money");

            builder.Property(e => e.ShippingDesc)
                .HasMaxLength(250)
                .IsUnicode(false);

            builder.Property(e => e.ShippingDiscount).HasColumnType("money");

            builder.Property(e => e.ShippingInclTax).HasColumnType("money");

            builder.Property(e => e.ShippingTax).HasColumnType("money");

            builder.Property(e => e.ShippingZipCode)
                .HasMaxLength(20)
                .IsUnicode(false);

            builder.Property(e => e.State)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            builder.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);

            builder.Property(e => e.StoreId).HasColumnName("StoreID");

            builder.Property(e => e.StoreName)
                .IsRequired()
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            builder.Property(e => e.Subtotal)
                .HasColumnType("money")
                .HasComment("Sum of all order items without tax and shipping");

            builder.Property(e => e.SubtotalInclTax)
                .HasColumnType("money")
                .HasComment("Sum of all order items with tax - no shipping");

            builder.Property(e => e.Time).HasColumnType("datetime");

            builder.Property(e => e.Total)
                .HasColumnType("money")
                .HasComment("Sum of all the items with tax and shipping");

            builder.Property(e => e.TotalPaid).HasColumnType("money");

            builder.Property(e => e.TotalQtyOrdered).HasColumnName("TotalQtyOrdered");

            builder.Property(e => e.UpdateShipstation).HasColumnName("updateShipstation");

            builder.Property(e => e.Weigth)
                .HasColumnType("decimal(10, 4)")
                .HasDefaultValueSql("((0))");
        }
    }
}
