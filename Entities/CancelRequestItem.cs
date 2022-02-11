using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Entities
{
    public class CancelRequestItem
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public int Quantity { get; set; }
        public int? CancelRequestOrderId { get; set; }

        public virtual CancelRequestOrder CancelRequestOrder { get; set; }
    }
}
