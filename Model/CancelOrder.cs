using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Model
{
    public class CancelOrder
    {
        public string orderNumber { get; set; } // Data
        public string userName { get; set; }  // Data
        public string prefix { get; set; }  // Data
        public string reason { get; set; }  
        public List<CancelItem> cancelItems { get; set; }
    }
}
