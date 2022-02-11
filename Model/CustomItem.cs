using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Model
{
    public class CustomItem
    {
        public int? customsItemId { get; set; }
        public string description { get; set; }
        public int? quantity { get; set; }
        public decimal? value { get; set; }
        public string harmonizedTariffCode { get; set; }
        public string countryOfOrigin { get; set; }
    }
}
