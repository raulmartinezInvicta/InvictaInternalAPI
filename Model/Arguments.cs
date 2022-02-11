using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Model
{
    public class Arguments
    {
        public Double shipping_amount { get; set; }
        public Double adjustment_positive { get; set; }
        public Double adjustment_negative { get; set; }
        public ExtensionAttributes extension_attributes { get; set; }
    }
}
