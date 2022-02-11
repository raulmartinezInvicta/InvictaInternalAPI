using System;

namespace InvictaInternalAPI.Entities
{
    public class CancelRequestStepStatus
    {
        public int Id { get; set; }
        public string Step { get; set; }
        public string StatusStep { get; set; }
        public string Link { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? CancelRequestOrderId { get; set; }
    }
}
