using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Model
{
    public class InsuranceOptions
    {
        public String provider { get; set; }
        public bool insureShipment { get; set; }
        public decimal? insuredValue { get; set; }
    }
}
