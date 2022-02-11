using System;
using System.Collections.Generic;

#nullable disable

namespace InvictaInternalAPI.Entities
{
    public partial class ECommerceOrder
    {
        
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public int OrderId { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime Time { get; set; }
        public decimal Subtotal { get; set; }
        public decimal SubtotalInclTax { get; set; }
        public decimal Total { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal? DiscountAmount { get; set; }
        public string DiscountDesc { get; set; }
        public string ShippingDesc { get; set; }
        public decimal? ShippingAmount { get; set; }
        public decimal ShippingTax { get; set; }
        public decimal ShippingDiscount { get; set; }
        public decimal ShippingInclTax { get; set; }
        public decimal SalesTax { get; set; }
        public byte[] DbtimeStamp { get; set; }
        public string State { get; set; }
        public string Status { get; set; }
        public string ShippingZipCode { get; set; }
        public string ShippingAddressType { get; set; }
        public string PaymentCctype { get; set; }
        public decimal? PaymentAmountOrdered { get; set; }
        public int? CustomerId { get; set; }
        public int? CompanyId { get; set; }
        public string FedexTrackingNo { get; set; }
        public byte UpdateShipstation { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public int TotalQtyOrdered { get; set; }
        public decimal? Weigth { get; set; }
        public string OrderCurrencyCode { get; set; }
        public byte IsPendingForwarding { get; set; }

    }
}
