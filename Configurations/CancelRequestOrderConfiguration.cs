using InvictaInternalAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvictaInternalAPI.Configurations
{
    public class CancelRequestOrderConfiguration : IEntityTypeConfiguration<CancelRequestOrder>
    {
        public void Configure(EntityTypeBuilder<CancelRequestOrder> builder)
        {
            builder.ToTable("CancelRequestOrder");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("ID");

            builder.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");

            builder.Property(e => e.OrderNumber).HasColumnName("orderNumber");

            builder.Property(e => e.Prefix)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("prefix");

            builder.Property(e => e.Reason)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("reason");

            builder.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("userName");

            builder.Property(e => e.CreditMemo)
                .IsRequired()
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("creditMemo");

        }
    }
}
