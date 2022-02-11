using System;
using System.Collections.Generic;

#nullable disable

namespace InvictaInternalAPI.Entities
{
    public partial class AspNetActivityLog
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Process { get; set; }
        public string Result { get; set; }
    }
}
