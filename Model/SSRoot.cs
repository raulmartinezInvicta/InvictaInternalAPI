using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Model
{
    public class SSRoot
    {
        public List<SSOrder> orders { get; set; }
        public int total { get; set; }
        public int page { get; set; }
        public int pages { get; set; }
    }
}
