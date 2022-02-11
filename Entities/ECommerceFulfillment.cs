using System;
using System.Collections.Generic;

#nullable disable

namespace InvictaInternalAPI.Entities
{
    public partial class ECommerceFulfillment
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public string ItemLookupCode { get; set; }
        public int Quantity { get; set; }
        public string Location { get; set; }
        public int? WasProcessed { get; set; }
        public int? Status { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateProcessed { get; set; }
        public byte? AutoCreated { get; set; }
        public int? InvAdjBatch { get; set; }
        public int? ECommerceOrderEntryId { get; set; }
        public int? OnAllocationReport { get; set; }
        public int IsConsOrdered { get; set; }
        public int ConsBatchId { get; set; }
        public int IsTaggedForPicking { get; set; }
        public long ECommerceShipstationOrderId { get; set; }
        public byte WasShipped { get; set; }
        public long SsShipmentId { get; set; }
        public DateTime? DateShipped { get; set; }
        public long ECommerceFulfillmentIdsource { get; set; }
        public int SourceWebsite { get; set; }
    }
}
