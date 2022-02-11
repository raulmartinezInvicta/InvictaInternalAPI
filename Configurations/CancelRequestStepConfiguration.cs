using InvictaInternalAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvictaInternalAPI.Configurations
{
    public class CancelRequestStepConfiguration : IEntityTypeConfiguration<CancelRequestStep>
    {
        public void Configure(EntityTypeBuilder<CancelRequestStep> builder)
        {
            builder.ToTable("CancelRequestSteps");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasColumnName("ID");

            builder.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updatedDate");

            builder.Property(e => e.Link)
                .IsRequired()
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("link");

            builder.Property(e => e.UpdatedBy)
                .IsRequired()
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("updatedBy");

            builder.Property(e => e.CancelRequestOrderId).HasColumnName("cancelRequestOrderID");

            builder.Property(e => e.StatusStep).HasColumnName("statusStep");

            builder.Property(e => e.Step)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("step");

            builder.HasOne(d => d.CancelRequestOrder)
                    .WithMany(p => p.CancelRequestSteps)
                    .HasForeignKey(d => d.CancelRequestOrderId)
                    .HasConstraintName("FK__CancelReq__cance__76EBA2E9");

        }
    }
}
