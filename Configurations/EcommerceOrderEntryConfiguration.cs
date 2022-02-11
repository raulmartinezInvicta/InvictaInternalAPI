using InvictaInternalAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvictaInternalAPI.Configurations
{
    public class EcommerceOrderEntryConfiguration : IEntityTypeConfiguration<ECommerceOrderEntry>
    {
        public void Configure(EntityTypeBuilder<ECommerceOrderEntry> builder)
        {
            builder.ToTable("eCommerceOrderEntry");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("ID");

            builder.Property(e => e.AddedIntoFulfillment).HasColumnName("addedIntoFulfillment");

            builder.Property(e => e.DbtimeStamp)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasColumnName("DBTimeStamp");

            builder.Property(e => e.DiscountAmount).HasColumnType("money");

            builder.Property(e => e.ECommerceOrderId).HasColumnName("eCommerceOrderID");

            builder.Property(e => e.FullPrice).HasColumnType("money");

            builder.Property(e => e.ItemId).HasColumnName("ItemID");

            builder.Property(e => e.ItemLookupCode)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            builder.Property(e => e.OrderNumber)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.ParentItemId).HasColumnName("ParentItemID");

            builder.Property(e => e.Price).HasColumnType("money");

            builder.Property(e => e.ProductId).HasColumnName("ProductID");

            builder.Property(e => e.ProductType)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValueSql("('')");

            builder.Property(e => e.QtyShipped).HasColumnName("QtyShipped");

            builder.Property(e => e.QtyOrdered).HasColumnName("QtyOrdered");

            builder.Property(e => e.QtyCancelled).HasColumnName("QtyCancelled");

            builder.Property(e => e.SimpleProdLineNo).HasColumnName("SimpleProdLineNo");

            builder.Property(e => e.QtyRefunded).HasColumnName("QtyRefunded");

            builder.Property(e => e.RefundedAmount).HasColumnType("money");

            builder.Property(e => e.RowTotal).HasColumnType("money");

            builder.Property(e => e.RowTotalInclTax).HasColumnType("money");

            builder.Property(e => e.SalesTax).HasColumnType("money");

            builder.Property(e => e.SourceCode)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValueSql("('default')");

            builder.Property(e => e.WasForwarded).HasColumnName("wasForwarded");

            builder.Property(e => e.Weight).HasColumnType("decimal(7, 4)");

        }
    }
}
