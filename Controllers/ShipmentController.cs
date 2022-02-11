using InvictaInternalAPI.Context;
using InvictaInternalAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Controllers
{
    //Update

    [Route("upload/shipping")]
    [ApiController]
    public class ShippingController : ControllerBase
    {

        private readonly InvictaAUXContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ShippingController> _logger;

        public ShippingController ( IConfiguration configuration, InvictaAUXContext context, ILogger<ShippingController> logger) {
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }


        [HttpPost]
        public async Task<ActionResult> PostShipment(int supplierID, ShipmentRoot _shipmentRoot)
        {

            _logger.LogInformation("Executing endpoint PostShipment/ShippingController");
            //Search or Create Hdr ShippersConfirmation
            var shiphdr =  _context.ShippersConfirmation.FirstOrDefault(hdr => hdr.OrderNumber.Equals(_shipmentRoot.orderNumber) && hdr.SupplierID == supplierID);
            if (shiphdr==null) {

                var newshiphdr = new ShippersConfirmation()
                {
                    OrderNumber = _shipmentRoot.orderNumber,
                    CompanyId = _shipmentRoot.companyId,
                    CustomerNumber = _shipmentRoot.customerNumber,
                    SupplierID = supplierID,
                    isNewEntry = 1,
                    OrderDate = DateTime.Now
                };

                _context.ShippersConfirmation.Add(newshiphdr);
                _context.SaveChanges();
                shiphdr =  _context.ShippersConfirmation.FirstOrDefault(hdr => hdr.OrderNumber.Equals(_shipmentRoot.orderNumber));
                _logger.LogInformation("ShipConfirmation record created...ID:" + shiphdr.ID);
            }


            foreach(var row in _shipmentRoot.details) {

                

                //Create missing records
                int ReturnLabel = 0;
                if (row.prePaidReturnLabelUsed)
                {
                    ReturnLabel = 1;
                }
                _logger.LogInformation("Alternate Part Number for: " + row.itemNumber + " -> " + GetAlternamePartNumber(_shipmentRoot.orderNumber, row.itemNumber));
                _logger.LogInformation("Line Number for " + row.itemNumber + " " + GetLineNumber(_shipmentRoot.orderNumber, row.itemNumber, row.lineNumber));

                var newrow = new ShippersConfirmationEntry() {
                    LineNumber=GetLineNumber(_shipmentRoot.orderNumber,row.itemNumber,row.lineNumber),
                    ItemCode=row.itemNumber,
                    OrderedQty=row.orderedQuantity,
                    ShippedQty= row.shippedQuantity,
                    CancelledQty=row.canceledQuantity,
                    TrackingNumber=row.trackingNumber,
                    ActualShippedDate = row.shippedDate,
                    Carrier=row.carrier,
                    ShipConfirmationID=shiphdr.ID,
                    prePaidReturnLabelCost=row.prePaidReturnLabelCost,
                    prePaidReturnLabelUsed= (byte?)ReturnLabel,
                    CompanyId=row.companyId,
                    ItemID=0,
                    ShipmentID=_shipmentRoot.mageShipmentId ?? 0,

                };
                _context.ShippersConfirmationEntry.Add(newrow);

                

                //Void records if applicable
                if (_shipmentRoot.voidTracking != null)  {
                    foreach (string voidtrac in _shipmentRoot.voidTracking) {
                        var voidreclist = await _context.ShippersConfirmationEntry.Where(dtl => dtl.ShipConfirmationID == shiphdr.ID && dtl.ItemCode == row.itemNumber && dtl.TrackingNumber == voidtrac ).ToListAsync();
                        foreach (var voidrec in voidreclist){
                            voidrec.TrackingNumber = "VOID "+voidrec.TrackingNumber;
                            _context.ShippersConfirmationEntry.Update(voidrec);
                        }                        
                    }
                }
                _logger.LogInformation("Ship Confirmation Entry Processed...ShipConfirmationID:" + shiphdr.ID);
                _context.SaveChanges();

            }

            var fullUpdate = UpdateFulfillment(_shipmentRoot);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
            DataLogJSON json = new DataLogJSON(){
                AddedDate=DateTime.Now,
                JSON = System.Text.Json.JsonSerializer.Serialize(_shipmentRoot, options),
                PayloadType=2,
                urlString=GetRawUrl(Request),
                responseString="Saved Successfully",
                EntityID= (int)shiphdr.ID,
                wasProcessed=1
            };
            _context.DataLogJSON.Add(json);
            _context.SaveChanges();
            _logger.LogInformation("Completed process");
            return StatusCode(StatusCodes.Status202Accepted);

        }

        [NonAction]
        public static string GetRawUrl(HttpRequest request)
        {
            var httpContext = request.HttpContext;

            var requestFeature = httpContext.Features.Get<IHttpRequestFeature>();

            return requestFeature.RawTarget;
        }

        [NonAction]
        public bool UpdateFulfillment(ShipmentRoot _shipmentRoot)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            foreach(var row in _shipmentRoot.details)
            {

                var sku = row.itemNumber;
                if (sku.Substring(0,4).Equals("AIC-")){
                    sku = row.itemNumber.Replace("AIC-","");
                } else {
                    sku = GetAlternamePartNumber(_shipmentRoot.orderNumber,row.itemNumber);
                }
                

                string sql = String.Format(@" update top({0}) eCommerceFulfillment set eCommerceShipstationOrderID = '{1}',  
                                              wasShipped = 1, ssShipmentID = '{2}', dateShipped='{3}' where eCommerceShipStationOrderID <> 0 and ssShipmentId=0 and wasShipped = 0
                                              and OrderNo = '{4}' and ItemLookUpCode = '{5}' and (select count(*) from eCommerceFulfillment where OrderNo = '{4}'
                                              and ItemLookUpCode = '{5}'  and wasShipped = 0) >= {0} ", row.shippedQuantity, _shipmentRoot.orderId, _shipmentRoot.shipmentId, row.shippedDate, _shipmentRoot.orderNumber, sku);

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnectionInvicta")))
                {
                    connection.Open();
                    using (SqlCommand command2 = new SqlCommand(sql, connection))
                    {
                        try
                        {
                            command2.ExecuteNonQuery();
                        }
                        catch (SqlException e)
                        {
                            return false;
                        }

                    }

                }

            }
            _logger.LogInformation("Fulfillment updated..." + _shipmentRoot.orderNumber);
            return true;

        }

        [NonAction]
        public string GetAlternamePartNumber(string orderNumber, string partNumber)
        {

            string altPartNumber = partNumber;
            try {
                
                
                string sql = String.Format(@"select eCommerceOrderEntry.ItemLookupCode
                            from eCommerceOrderEntry,eCommerceFulfillment
                            where eCommerceOrderEntry.id =  eCommerceFulfillment.eCommerceOrderEntryID  
                            AND eCommerceFulfillment.OrderNo= '{0}'
                            AND eCommerceFulfillment.ItemLookupCode = '{1}'
                            AND eCommerceOrderEntry.ItemLookupCode<>eCommerceFulfillment.ItemLookupCode",orderNumber, partNumber);
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnectionInvicta")))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(sql, connection)) {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                altPartNumber = reader.GetString(0);
                                return altPartNumber;
                            }

                        }
                    }

                }

                } catch {

                    return altPartNumber;
                }

            return altPartNumber;    
        }

        [NonAction]
        public Int32 GetLineNumber( string orderNumber, string itemNumber, Int32 lineNo) {

            try {

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnectionInvicta")))
                {
                    connection.Open();
                    string SQLstr = String.Format("select SimpleProdLineNo from eCommerceOrderEntry where OrderNumber = '{0}' and ItemLookupCode = '{1}' and ProductType='simple' ", orderNumber,GetAlternamePartNumber(orderNumber,itemNumber));
                    using (SqlCommand cmd = new SqlCommand(SQLstr, connection)) {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lineNo = reader.GetInt32(0);
                                return lineNo;
                            }

                        }
                    }

                }

                } catch {

                    return lineNo;
                }

            return lineNo;    
        }       

    }


    public class ShipmentDetail
    {
        public int lineNumber { get; set; }
        public string itemNumber { get; set; }
        public int orderedQuantity { get; set; }
        public int shippedQuantity { get; set; }
        public int canceledQuantity { get; set; }
        public DateTime shippedDate { get; set; }
        public string carrier { get; set; }
        public string trackingNumber { get; set; }
        public bool prePaidReturnLabelUsed { get; set; }
        public Nullable<decimal> prePaidReturnLabelCost { get; set; }   
        public int companyId {get;set;} 

    }

    public class ShipmentRoot
    {
        public string orderNumber { get; set; }
        public string customerNumber { get; set; }
        public DateTime orderDate { get; set; }
        public int companyId { get; set; }
        public int orderId{get;set;}  //Shipstation Order Id
        public int shipmentId{get;set;}  //Shipstation Shipment Id
        public Nullable<int> mageShipmentId{get;set;} //Magento Shipment Id
        public List<string> voidTracking{get;set;} //Shipstation void tracking linked to orderId 
        public List<ShipmentDetail> details { get; set; }
    }

}
