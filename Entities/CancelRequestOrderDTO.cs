using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Entities
{
    public class CancelRequestOrderDTO
    {
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        public string UserName { get; set; }
        public string Prefix { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreditMemo { get; set; }
        public bool Status { get; set; }
        public bool Complete { get; set; }
    }
}
