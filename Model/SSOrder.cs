using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Model
{
    public class SSOrder
    {
        public int orderId { get; set; }
        public string orderNumber { get; set; }
        public string orderKey { get; set; }
        public DateTime orderDate { get; set; }
        public DateTime createDate { get; set; }
        public DateTime modifyDate { get; set; }
        public DateTime? paymentDate { get; set; }
        public object shipByDate { get; set; }
        public string orderStatus { get; set; }
        public int? customerId { get; set; }
        public string customerUsername { get; set; }
        public string customerEmail { get; set; }
        public ShipTo shipTo { get; set; }
        public BillTo billTo { get; set; }
        public List<SSItem> items { get; set; }
        public decimal? orderTotal { get; set; }
        public decimal? amountPaid { get; set; }
        public decimal? taxAmount { get; set; }
        public decimal? shippingAmount { get; set; }
        public string customerNotes { get; set; }
        public string internalNotes { get; set; }
        public bool gift { get; set; }
        public object giftMessage { get; set; }
        public object paymentMethod { get; set; }
        public string requestedShippingService { get; set; }
        public string carrierCode { get; set; }
        public string serviceCode { get; set; }
        public string packageCode { get; set; }
        public string confirmation { get; set; }
        public string shipDate { get; set; }
        public object holdUntilDate { get; set; }
        public Weight weight { get; set; }
        public Dimensions dimensions { get; set; }
        public InsuranceOptions insuranceOptions { get; set; }
        public AdvancedOptions advancedOptions { get; set; }
        public InternationalOptions internationalOptions { get; set; }
        public object tagIds { get; set; }
        public object userId { get; set; }
        public bool externallyFulfilled { get; set; }
        public object externallyFulfilledBy { get; set; }
        public object labelMessages { get; set; }
    }
}
