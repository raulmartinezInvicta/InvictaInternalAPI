using InvictaInternalAPI.Context;
using InvictaInternalAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Controllers
{


    [Route("upload/tco/shipping")]
    [ApiController]
    public class ShippingTCOController : ControllerBase
    {
        //Change Context
        private readonly MerlinContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ShippingTCOController> _logger;

        public ShippingTCOController ( IConfiguration configuration, MerlinContext context, ILogger<ShippingTCOController> logger) {
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }


        [HttpPost]
        public async Task<ActionResult> PostShipment(int supplierID, ShipmentRoot _shipmentRoot)
        {


            _logger.LogInformation("Executing endpoint PostShipment/ShippingTCOController");
            var shiphdr =  _context.ShippersConfirmation.FirstOrDefault(hdr => hdr.OrderNumber.Equals(_shipmentRoot.orderNumber) && hdr.SupplierID == supplierID);
            if (shiphdr==null) {
                _logger.LogInformation($"ShippersConfirmation found : {shiphdr.OrderNumber}");
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

                string sku = GetAlternamePartNumber(_shipmentRoot.orderNumber,row.itemNumber,supplierID);
                int lineNumber = GetLineNumber(_shipmentRoot.orderNumber,sku,row.lineNumber,supplierID);
                _logger.LogInformation("Alternate Part Number for: " + row.itemNumber + " -> " + sku);
                _logger.LogInformation("Line Number for " + row.itemNumber + " " + lineNumber);
                var newrow = new ShippersConfirmationEntry() {
                    LineNumber=lineNumber,
                    ItemCode=sku,
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
                _context.SaveChanges();

            }

            var fullUpdate = UpdateFulfillment(_shipmentRoot,supplierID);

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
        public bool UpdateFulfillment(ShipmentRoot _shipmentRoot, int supplierID)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            foreach(var row in _shipmentRoot.details)
            {

                string sku = GetAlternamePartNumber(_shipmentRoot.orderNumber,row.itemNumber, supplierID);
                
                string sql = String.Format(@" update top({0}) eCommerceFulfillment set eCommerceShipstationOrderID = '{1}',  
                                              wasShipped = 1, ssShipmentID = '{2}', dateShipped='{3}' where eCommerceShipStationOrderID <> 0 and ssShipmentId=0 and wasShipped = 0
                                              and OrderNo = '{4}' and ItemLookUpCode = '{5}' and (select count(*) from eCommerceFulfillment where OrderNo = '{4}'
                                              and ItemLookUpCode = '{5}'  and wasShipped = 0) >= {0} ", row.shippedQuantity, _shipmentRoot.orderId, _shipmentRoot.shipmentId, row.shippedDate, _shipmentRoot.orderNumber, sku);

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnectionMerlin")))
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
                            _logger.LogInformation("Fulfillment update failed..." + _shipmentRoot.orderNumber + " " + e.ToString());
                            return false;
                        }

                    }

                }

            }
            _logger.LogInformation("Fulfillment updated..." + _shipmentRoot.orderNumber);
            return true;

        }

        [NonAction]
        public string GetAlternamePartNumber(string orderNumber, string partNumber, int supplierID)
        {

            string altPartNumber = partNumber;
            try {
                
                
                string sql = String.Format(@"select eCommerceOrderEntry.ItemLookupCode from eCommerceOrderEntry
                                            where (eCommerceOrderEntry.ItemLookupCode = CONCAT((select ItemNumberPrefix from ShippersSupplier where id = {2}),'-','{1}') or
                                            eCommerceOrderEntry.ItemLookupCode = '{1}' )
                                            and eCommerceOrderEntry.OrderNumber = '{0}'",orderNumber, partNumber, supplierID);

                Console.WriteLine("sql:"+sql);
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnectionMerlin")))
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

                } catch (Exception e) {
                    
                    Console.WriteLine(e.ToString());
                    return altPartNumber;
                }

            return altPartNumber;    
        }

        [NonAction]
        public Int32 GetLineNumber( string orderNumber, string itemNumber, Int32 lineNo,int supplierID) {

            try {

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnectionMerlin")))
                {
                    connection.Open();
                    string SQLstr = String.Format("select SimpleProdLineNo from eCommerceOrderEntry where OrderNumber = '{0}' and ItemLookupCode = '{1}' and ProductType='simple' ", orderNumber,itemNumber);
                    Console.WriteLine("sql"+SQLstr);
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

}