using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Model
{
    public class Root
    {
        public List<Item> items { get; set; }
        public bool notify { get; set; }
        public bool isOnline { get; set; }
        public bool appendComment { get; set; }
        public Comment comment { get; set; }
        public Arguments arguments { get; set; }
    }
}
