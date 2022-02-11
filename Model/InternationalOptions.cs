using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Model
{
    public class InternationalOptions
    {
        public string contents { get; set; }
        public List<CustomItem> customsItems { get; set; }
        public Object nonDelivery { get; set; }
    }
}
