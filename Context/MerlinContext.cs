using InvictaInternalAPI.Configurations;
using InvictaInternalAPI.Entities;
using InvictaInternalAPI.Model;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace InvictaInternalAPI.Context
{
    public partial class MerlinContext : DbContext
    {
        public MerlinContext()
        {
        }

        public MerlinContext(DbContextOptions<MerlinContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AspNetActivityLog> AspNetActivityLogs { get; set; }
        public virtual DbSet<CancelRequestOrder> CancelRequestOrders { get; set; }
        public virtual DbSet<CancelRequestItem> CancelRequestItems { get; set; }
        public virtual DbSet<CancelRequestStep> CancelRequestSteps { get; set; }
        public virtual DbSet<ECommerceFulfillment> ECommerceFulfillments { get; set; }
        public DbSet<ShippersConfirmation> ShippersConfirmation { get; set; }
        public DbSet<ShippersConfirmationEntry> ShippersConfirmationEntry { get; set; }
        public virtual DbSet<ECommerceOrderEntry> ECommerceOrderEntries { get; set; }
        public virtual DbSet<ECommerceOrder> ECommerceOrders { get; set; }
        public DbSet<DataLogJSON> DataLogJSON { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");
            modelBuilder.ApplyConfiguration(new AspNetActivityLogConfiguration());
            modelBuilder.ApplyConfiguration(new CancelRequestOrderConfiguration());
            modelBuilder.ApplyConfiguration(new CancelRequestItemConfiguration());
            modelBuilder.ApplyConfiguration(new CancelRequestStepConfiguration());
            modelBuilder.ApplyConfiguration(new EcommerceFulfillmentConfiguration());
            modelBuilder.ApplyConfiguration(new EcommerceOrderEntryConfiguration());
            modelBuilder.ApplyConfiguration(new EcommerceOrderConfiguration());


        }
    }
}
