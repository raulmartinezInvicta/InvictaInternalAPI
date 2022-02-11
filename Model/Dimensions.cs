using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Model
{
    public class Dimensions
    {
        public String units { get; set; }
        public decimal? length { get; set; }
        public decimal? width { get; set; }
        public decimal? height { get; set; }
    }
}
