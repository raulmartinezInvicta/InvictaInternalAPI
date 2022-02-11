using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Entities
{
    public class MasterItem
    {
        public int QtyAvailable { get; set; }
        public List<ItemMagento> Items { get; set; }
        public double shipping_amout { get; set; }
    }
}
