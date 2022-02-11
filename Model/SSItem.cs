using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Model
{
    public class SSItem
    {
        public String lineItemKey { get; set; }
        public String sku { get; set; }
        public String name { get; set; }
        public Object imageUrl { get; set; }
        public Weight weight { get; set; }
        public int? quantity { get; set; }
        public decimal? unitPrice { get; set; }
        public decimal? taxAmount { get; set; }
        public decimal? shippingAmount { get; set; }
        public String warehouseLocation { get; set; }
        public List<Option> options { get; set; }
        public int? productId { get; set; }
        public String fulfillmentSku { get; set; }
        public bool adjustment { get; set; }
        public String upc { get; set; }
    }
}
