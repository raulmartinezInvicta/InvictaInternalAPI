using System;
using System.Collections.Generic;

#nullable disable

namespace InvictaInternalAPI.Entities
{
    public partial class CancelRequestStep 
    {
        public int Id { get; set; }
        public string Step { get; set; }
        public bool StatusStep { get; set; }
        public string Link { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? CancelRequestOrderId { get; set; }

        public virtual CancelRequestOrder CancelRequestOrder { get; set; }

        
    }
}
