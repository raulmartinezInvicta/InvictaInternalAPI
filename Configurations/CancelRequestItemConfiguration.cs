using InvictaInternalAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvictaInternalAPI.Configurations
{
    public class CancelRequestItemConfiguration : IEntityTypeConfiguration<CancelRequestItem>
    {
        public void Configure(EntityTypeBuilder<CancelRequestItem> builder)
        {
            builder.ToTable("CancelRequestItem");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("ID");

            builder.Property(e => e.Quantity).HasColumnName("quantity");

            builder.Property(e => e.CancelRequestOrderId).HasColumnName("cancelRequestOrderID");

            builder.Property(e => e.Sku)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("sku");

            builder.HasOne(d => d.CancelRequestOrder)
                    .WithMany(p => p.CancelRequestItems)
                    .HasForeignKey(d => d.CancelRequestOrderId)
                    .HasConstraintName("FK__CancelReq__cance__75F77EB0");
        }
    }
}
