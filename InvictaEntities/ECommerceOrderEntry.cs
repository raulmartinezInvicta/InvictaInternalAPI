using System;
using System.Collections.Generic;

#nullable disable

namespace InvictaInternalAPI.InvictaEntities
{
    public partial class ECommerceOrderEntry
    {
        public int Id { get; set; }
        public decimal FullPrice { get; set; }
        public int? OrderNumber { get; set; }
        public string ItemLookupCode { get; set; }
        public decimal Price { get; set; }
        public int QtyOrdered { get; set; }
        public int? QtyShipped { get; set; }
        public int? QtyCancelled { get; set; }
        public int? QtyRefunded { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? RefundedAmount { get; set; }
        public byte[] DbtimeStamp { get; set; }
        public decimal SalesTax { get; set; }
        public decimal? RowTotal { get; set; }
        public byte? AddedIntoFulfillment { get; set; }
        public int? ItemId { get; set; }
        public int? ProductId { get; set; }
        public int? ParentItemId { get; set; }
        public string Name { get; set; }
        public decimal? RowTotalInclTax { get; set; }
        public string ProductType { get; set; }
        public decimal? Weight { get; set; }
        public int? SimpleProdLineNo { get; set; }
        public string SourceCode { get; set; }
        public int? ECommerceOrderId { get; set; }
        public byte? WasForwarded { get; set; }
    }
}
