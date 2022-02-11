using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Entities
{
    public class LinesItems
    {
        public string UserName { get; set; }
        public int OrderNumber { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public string Prefix { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
        public string RowTotalInclTax { get; set; }
        public string Method { get; set; }
    }
}
