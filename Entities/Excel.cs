using System;

namespace InvictaInternalAPI.Entities
{
    public class Excel
    {
        public DateTime Date { get; set; }
        public string OrderNumber { get; set; }
        public string Sku { get; set; }
        public string Reason { get; set; }
        public string Method { get; set; }
        public string CCType { get; set; }
        public string Quantity { get; set; }
        public string Prefix { get; set; }
        public string Amount { get; set; }
        public bool Complete { get; set; }
    }
}
