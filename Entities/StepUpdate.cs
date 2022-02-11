using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Entities
{
    public class StepUpdate
    {
        public int cancelRequestID { get; set; }
        public string step { get; set; }
        public bool statusStep { get; set; }
        public string updatedBy { get; set; }
    }
}
