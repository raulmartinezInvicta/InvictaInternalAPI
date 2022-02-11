using InvictaInternalAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvictaInternalAPI.Configurations
{
    public class EcommerceFulfillmentConfiguration : IEntityTypeConfiguration<ECommerceFulfillment>
    {
        public void Configure(EntityTypeBuilder<ECommerceFulfillment> builder)
        {
            builder.ToTable("eCommerceFulfillment");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("ID");

            builder.Property(e => e.ConsBatchId).HasColumnName("ConsBatchID");

            builder.Property(e => e.DateCreated).HasColumnType("datetime");

            builder.Property(e => e.InvAdjBatch).HasColumnName("InvAdjBatch");

            builder.Property(e => e.Status).HasColumnName("Status");

            builder.Property(e => e.DateProcessed).HasColumnType("datetime");

            builder.Property(e => e.AutoCreated).HasColumnName("AutoCreated");

            builder.Property(e => e.DateShipped)
                .HasColumnType("datetime")
                .HasColumnName("dateShipped");

            builder.Property(e => e.ECommerceFulfillmentIdsource).HasColumnName("eCommerceFulfillmentIDSource");

            builder.Property(e => e.ECommerceOrderEntryId).HasColumnName("eCommerceOrderEntryID");

            builder.Property(e => e.ECommerceShipstationOrderId).HasColumnName("eCommerceShipstationOrderID");

            builder.Property(e => e.IsConsOrdered).HasColumnName("isConsOrdered");

            builder.Property(e => e.ConsBatchId).HasColumnName("ConsBatchId");

            builder.Property(e => e.IsTaggedForPicking).HasColumnName("isTaggedForPicking");

            builder.Property(e => e.Quantity).HasColumnName("Quantity");

            builder.Property(e => e.ItemLookupCode)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.Location)
                .HasMaxLength(10)
                .IsUnicode(false);

            builder.Property(e => e.OnAllocationReport).HasColumnName("onAllocationReport");

            builder.Property(e => e.OrderNo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);

            builder.Property(e => e.SsShipmentId).HasColumnName("ssShipmentID");

            builder.Property(e => e.WasProcessed).HasColumnName("wasProcessed");

            builder.Property(e => e.WasShipped).HasColumnName("wasShipped");

            builder.Property(e => e.ECommerceFulfillmentIdsource).HasColumnName("eCommerceFulfillmentIDSource");

            builder.Property(e => e.SourceWebsite).HasColumnName("SourceWebsite");
        }
    }
}
