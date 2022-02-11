using System;
using System.Collections.Generic;

#nullable disable

namespace InvictaInternalAPI.Entities
{
    public partial class CancelRequestOrder
    {
        public CancelRequestOrder()
        {
            CancelRequestItems = new HashSet<CancelRequestItem>();
            CancelRequestSteps = new HashSet<CancelRequestStep>();
        }
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        public string UserName { get; set; }
        public string Prefix { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreditMemo { get; set; }

        public virtual ICollection<CancelRequestItem> CancelRequestItems { get; set; }
        public virtual ICollection<CancelRequestStep> CancelRequestSteps { get; set; }

        
    }
}
