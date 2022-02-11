using System;

namespace InvictaInternalAPI.Model
{


    public class DataLogJSON {
        public int ID { get; set; }

        public int PayloadType { get; set; }

        public string JSON { get; set; }

        public DateTime AddedDate { get; set; }

        public int wasProcessed { get; set; }

        public string responseString { get; set; }

        public string urlString { get; set; }

        public int EntityID { get; set; }

    }


}
