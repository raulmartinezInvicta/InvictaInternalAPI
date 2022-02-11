using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Model
{
    public class AdvancedOptions
    {
        public int? warehouseId { get; set; }
        public bool nonMachinable { get; set; }
        public bool saturdayDelivery { get; set; }
        public bool containsAlcohol { get; set; }
        public bool mergedOrSplit { get; set; }
        public List<int> mergedIds { get; set; }
        public int? parentId { get; set; }
        public int? storeId { get; set; }
        public String customField1 { get; set; }
        public String customField2 { get; set; }
        public String customField3 { get; set; }
        public String source { get; set; }
        public String billToParty { get; set; }
        public String billToAccount { get; set; }
        public String billToPostalCode { get; set; }
        public String billToCountryCode { get; set; }
        public String billToMyOtherAccount { get; set; }
    }
}
