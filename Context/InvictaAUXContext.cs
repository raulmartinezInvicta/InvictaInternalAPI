using InvictaInternalAPI.InvictaEntities;
using InvictaInternalAPI.Model;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace InvictaInternalAPI.Context
{
    public partial class InvictaAUXContext : DbContext
    {
        public InvictaAUXContext()
        {
        }

        public InvictaAUXContext(DbContextOptions<InvictaAUXContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ECommerceFulfillment> ECommerceFulfillments { get; set; }
        public DbSet<ShippersConfirmation> ShippersConfirmation { get; set; }
        public DbSet<ShippersConfirmationEntry> ShippersConfirmationEntry { get; set; }
        public virtual DbSet<ECommerceOrderEntry> ECommerceOrderEntries { get; set; }
        public virtual DbSet<ECommerceOrder> ECommerceOrders { get; set; }
        public DbSet<DataLogJSON> DataLogJSON { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");
   
            modelBuilder.Entity<ECommerceFulfillment>(entity =>
            {
                entity.ToTable("eCommerceFulfillment");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AutoCreated).HasDefaultValueSql("((0))");

                entity.Property(e => e.ConsBatchId).HasColumnName("ConsBatchID");

                entity.Property(e => e.DateCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateProcessed).HasColumnType("datetime");

                entity.Property(e => e.DateShipped)
                    .HasColumnType("datetime")
                    .HasColumnName("dateShipped");

                entity.Property(e => e.SourceWebsite).HasColumnName("SourceWebsite");

                entity.Property(e => e.ECommerceFulfillmentIdsource).HasColumnName("eCommerceFulfillmentIDSource");

                entity.Property(e => e.ECommerceOrderEntryId)
                    .HasColumnName("eCommerceOrderEntryID")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ECommerceShipstationOrderId).HasColumnName("eCommerceShipstationOrderID");

                entity.Property(e => e.InvAdjBatch).HasDefaultValueSql("((-1))");

                entity.Property(e => e.InvHoldStatus)
                    .HasColumnName("invHoldStatus")
                    .HasDefaultValueSql("((0))")
                    .HasComment("Inventory Hold Status - 0: not processed / 1: processed no hold / 2: processed set to hold");

                entity.Property(e => e.IsConsOrdered).HasColumnName("isConsOrdered");

                entity.Property(e => e.IsTaggedForPicking).HasColumnName("isTaggedForPicking");

                entity.Property(e => e.ItemLookupCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Location)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('NONE')");

                entity.Property(e => e.OnAllocationReport)
                    .HasColumnName("onAllocationReport")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.OrderNo)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.SsShipmentId).HasColumnName("ssShipmentID");

                entity.Property(e => e.Status).HasDefaultValueSql("((0))");

                entity.Property(e => e.WasProcessed)
                    .HasColumnName("wasProcessed")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.WasShipped).HasColumnName("wasShipped");
            });

            modelBuilder.Entity<ECommerceOrderEntry>(entity =>
            {
                entity.ToTable("eCommerceOrderEntry");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AddedIntoFulfillment)
                    .HasColumnName("addedIntoFulfillment")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DbtimeStamp)
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .HasColumnName("DBTimeStamp");

                entity.Property(e => e.DiscountAmount)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ECommerceOrderId).HasColumnName("eCommerceOrderID");

                entity.Property(e => e.FullPrice).HasColumnType("money");

                entity.Property(e => e.ItemId).HasColumnName("ItemID");

                entity.Property(e => e.ItemLookupCode)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.QtyOrdered).HasColumnName("QtyOrdered");

                entity.Property(e => e.OrderNumber).HasDefaultValueSql("((0))");

                entity.Property(e => e.ParentItemId).HasColumnName("ParentItemID");

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.ProductType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.QtyCancelled).HasDefaultValueSql("((0))");

                entity.Property(e => e.QtyRefunded).HasDefaultValueSql("((0))");

                entity.Property(e => e.QtyShipped).HasDefaultValueSql("((0))");

                entity.Property(e => e.RefundedAmount).HasColumnType("money");

                entity.Property(e => e.RowTotal)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.RowTotalInclTax).HasColumnType("money");

                entity.Property(e => e.SalesTax).HasColumnType("money");

                entity.Property(e => e.SourceCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.WasForwarded).HasColumnName("wasForwarded");

                entity.Property(e => e.Weight).HasColumnType("decimal(7, 4)");

                entity.Property(e => e.SimpleProdLineNo).HasColumnName("SimpleProdLineNo");
            });

            modelBuilder.Entity<ECommerceOrder>(entity =>
            {
                entity.ToTable("eCommerceOrder");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.OrderNumber).HasColumnName("OrderNumber");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CompanyId)
                    .HasColumnName("CompanyID")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CustomerEmail)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

                entity.Property(e => e.DbtimeStamp)
                    .IsRowVersion()
                    .IsConcurrencyToken()
                    .HasColumnName("DBTimeStamp");

                entity.Property(e => e.DiscountAmount)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.DiscountDesc)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FedexTrackingNo)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.IsPendingForwarding).HasColumnName("isPendingForwarding");

                entity.Property(e => e.OrderCurrencyCode)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.PaymentAmountOrdered)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.PaymentCctype)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("PaymentCCType");

                entity.Property(e => e.SalesTax).HasColumnType("money");

                entity.Property(e => e.ShippingAddressType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ShippingAmount)
                    .HasColumnType("money")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ShippingDesc)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ShippingDiscount).HasColumnType("money");

                entity.Property(e => e.ShippingInclTax).HasColumnType("money");

                entity.Property(e => e.ShippingTax).HasColumnType("money");

                entity.Property(e => e.ShippingZipCode)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.State)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StoreId).HasColumnName("StoreID");

                entity.Property(e => e.StoreName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Subtotal).HasColumnType("money");

                entity.Property(e => e.SubtotalInclTax).HasColumnType("money");

                entity.Property(e => e.Time)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Total).HasColumnType("money");

                entity.Property(e => e.TotalPaid).HasColumnType("money");

                entity.Property(e => e.TotalQtyOrdered).HasColumnName("TotalQtyOrdered");

                entity.Property(e => e.UpdateShipstation).HasColumnName("updateShipstation");

                entity.Property(e => e.Weigth).HasColumnType("decimal(7, 4)");
            });
        }

        
    }
}
