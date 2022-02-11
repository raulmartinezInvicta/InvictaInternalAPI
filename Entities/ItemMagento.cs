using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Entities
{
    public class ItemMagento
    {
        public string Sku { get; set; }
        public int ordered { get; set; }
        public int cancelled_ship { get; set; }
    }
}
