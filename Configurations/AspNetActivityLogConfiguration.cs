using InvictaInternalAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvictaInternalAPI.Configurations
{
    public class AspNetActivityLogConfiguration : IEntityTypeConfiguration<AspNetActivityLog>
    {
        public void Configure(EntityTypeBuilder<AspNetActivityLog> builder)
        {
            builder.HasNoKey();

            builder.ToTable("AspNetActivityLog");

            builder.Property(e => e.Id).ValueGeneratedOnAdd();

            builder.Property(e => e.Process)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Result).HasMaxLength(200);

            builder.Property(e => e.TimeStamp)
                .HasColumnType("datetime")
                .HasDefaultValueSql("(getdate())");

            builder.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(256);
        }
    }
}
