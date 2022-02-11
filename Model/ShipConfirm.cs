using System;

namespace InvictaInternalAPI.Model
{

    public class ShippersConfirmation
    {
        public long ID { get; set; }
        public string OrderNumber { get; set; }
        public System.DateTime OrderDate { get; set; }
        public string CustomerNumber { get; set; }
        public int isNewEntry { get; set; }
        public int SupplierID { get; set; }
        public Nullable<int> CompanyId { get; set; }

    }

    public class ShippersConfirmationEntry
    {

        public long ID { get; set; }
        public int LineNumber { get; set; }
        public string ItemCode { get; set; }
        public int OrderedQty { get; set; }
        public int ShippedQty { get; set; }
        public int CancelledQty { get; set; }
        public Nullable<System.DateTime> ActualShippedDate { get; set; }
        public string Carrier { get; set; }
        public string TrackingNumber { get; set; }
        public Nullable<byte> prePaidReturnLabelUsed { get; set; }
        public Nullable<decimal> prePaidReturnLabelCost { get; set; }
        public long ShipConfirmationID { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public Nullable<int> ItemID { get; set; }
        public Nullable<int> ShipmentID { get; set; }        
    }


}