using InvictaInternalAPI.Controllers;
using InvictaInternalAPI.Entities;
using InvictaInternalAPI.Exceptions;
using InvictaInternalAPI.Interfaces;
using InvictaInternalAPI.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Services
{
    public class MagentoSupport : IMagentoSupport
    {
        private readonly ICancelService _cancel;
        private readonly ICancelInvictaService _cancelInvicta;
        private readonly IShipStationService _shipStation;
        private readonly ILogger<MagentoSupport> _logger;
        public MagentoSupport(ICancelService cancel, ILogger<MagentoSupport> logger, ICancelInvictaService cancelInvicta, IShipStationService shipStation)
        {
            _cancel = cancel;
            _cancelInvicta = cancelInvicta;
            _shipStation = shipStation;
            _logger = logger;
        }
        public static async Task<string> GetMagentoToken(string prefix, IConfiguration _configuration)
        {
            var uri = _configuration[prefix + ":baseurl"] + "integration/admin/token";
            var user = _configuration[prefix + ":user"];
            var password = _configuration[prefix + ":password"];
            Console.WriteLine("uri:" + uri);
            Uri u = new Uri(uri);
            var payload = "{\"username\": \"" + user + "\",\"password\": \"" + password + "\"}";

            HttpContent c = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");

            var t = await SendURI(u, c, null);
            return "{\"token\": " + t + ",\"url\": \"" + _configuration[prefix + ":baseurl"] + "orders\" }";
        }
        public  async Task<string> CancelOrder(String userName, String reason, String prefix, CancelOrder cancelOrder, IConfiguration _configuration,bool kind,CancelRequestOrder cancelRequestOrder,MasterItem master)   
        {


            var magentoTokenJson = JObject.Parse(await MagentoSupport.GetMagentoToken(prefix, _configuration));
            _logger.LogDebug("Extracting token from magento");
            var token = magentoTokenJson["token"].ToString();
            _logger.LogDebug($"Token : {token}");
            JsonDocument orderJson = await MagentoSupport.GetMagentoOrder(token, prefix, _configuration, cancelOrder.orderNumber);
            _logger.LogDebug("Successful magento order extraction");
            JsonElement rootOrder = orderJson.RootElement;
            JsonElement itemsElm = rootOrder.GetProperty("items");
            JsonElement itemElm = itemsElm[0];
            JsonElement orderIdElm = itemElm.GetProperty("entity_id");
            JsonElement status = itemElm.GetProperty("status");
            String order_id = orderIdElm.ToString();


            if(Convert.ToString(status).ToLower() == "canceled")
            {
                await _cancel.AddOrder(cancelRequestOrder);
                _logger.LogError($"The order {cancelOrder.orderNumber} has already been canceled");
                throw new BusinessException("The order has already been canceled");
            }

            JsonDocument invoiceJson = await MagentoSupport.GetMagentoInvoice(token, prefix, _configuration, order_id);
            var shipping_amount = CalculateShippingAmountCredit(master,cancelOrder.cancelItems);
            _logger.LogDebug($"Shipping amount calculated : {shipping_amount}");
            string creditMemo = await MagentoSupport.MagentoCreditMemoAsync(token, prefix, _configuration, invoiceJson, cancelOrder, userName, reason, shipping_amount);
            _logger.LogDebug($"Generated creditmemo : {creditMemo}");
            if (!String.IsNullOrEmpty(creditMemo))
            {
                cancelRequestOrder.CreditMemo = creditMemo;
                _logger.LogDebug("Updating creditmemo");
                var magento =  _cancel.UpdateStepGeneral(cancelRequestOrder, "Magento");


                if (kind == true)
                {
                    //ShipStationCancelController ssCtrl = new ShipStationCancelController(_configuration, _logger, _cancel);
                    //var shipStationResult = await ssCtrl.CancelShipStationOrder(cancelOrder);
                    _logger.LogDebug("Preparing to cancel in ShipStation");
                    var shipStationResult = await _shipStation.CancelShipStationOrder(cancelOrder);
                    if (shipStationResult)
                    {
                        _logger.LogDebug("Successful cancellation in ShipStation");
                        var shipStation = _cancel.UpdateStepGeneral(magento, "ShipStation");
                        await _cancel.AddOrder(shipStation);
                    }
                    else
                    {
                        _logger.LogDebug("Unsuccessful cancellation in ShipStation");
                        await _cancel.AddOrder(magento);
                    }
                    
                    
                }
                else
                {
                    await _cancel.AddOrder(magento);

                }

                var creditMemoObj = await MagentoSupport.GetMagentoCreditMemo(token, prefix, _configuration, creditMemo);
                _logger.LogDebug("Sending mail to customer");
                GmailSupport.SendCreditMemoEmail(prefix,reason,creditMemoObj,orderJson, shipping_amount);
            }
            else
            {
                cancelRequestOrder.CreditMemo = "";

                if (kind == true)
                {
                    //ShipStationCancelController ssCtrl = new ShipStationCancelController(_configuration, _logger, _cancel);
                    //var shipStationResult = await ssCtrl.CancelShipStationOrder(cancelOrder);
                    _logger.LogDebug("Preparing to cancel in ShipStation");
                    var shipStationResult = await _shipStation.CancelShipStationOrder(cancelOrder);
                    if (shipStationResult)
                    {
                        _logger.LogDebug("Successful cancellation in ShipStation");
                        var shipStation = _cancel.UpdateStepGeneral(cancelRequestOrder, "ShipStation");
                        await _cancel.AddOrder(shipStation);
                    }
                    else
                    {
                        _logger.LogDebug("Unsuccessful cancellation in ShipStation");
                        await _cancel.AddOrder(cancelRequestOrder);
                    }
                    
                }
                else
                {
                    await _cancel.AddOrder(cancelRequestOrder);
                }
            }
            return creditMemo;
        }


        public static async Task<string> SendURI(Uri u, HttpContent c, string token)
        {
            string email = "";
            var response = string.Empty;
            using (var client = new HttpClient())
            {
                Console.WriteLine("Token:" + token);
                Console.WriteLine("Uri:" + u.ToString());
                Console.WriteLine("Content:" + c.ToString());
                email = email + "Token:" + token + "\r\n";
                email = email + "Uri:" + u.ToString() + "\r\n";
                email = email + "Content:" + c.ToString() + "\r\n";

                if (!String.IsNullOrEmpty(token))
                {  //use the token as authorization if provided
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    Console.WriteLine("autorization added to client");
                }


                HttpResponseMessage result = await client.PostAsync(u, c);
                Console.WriteLine("Result:" + result.Headers.ToString());
                Console.WriteLine("Response Code:" + result.StatusCode);
                email = email + "Result:" + result.Headers.ToString() + "\r\n";
                email = email + "Response Code:" + result.StatusCode + "\r\n";
                if (result.IsSuccessStatusCode)
                {
                    response = await result.Content.ReadAsStringAsync();

                }

            }

            return response;
        }

        private double CalculateShippingAmountCredit(MasterItem master, List<CancelItem> items)
        {
            if(master.shipping_amout == 0)
            {
                return 0;
            }
            else
            {
                int totalordered = 0;
                int totalcancelled = 0;
                foreach(ItemMagento im in master.Items)
                {
                    totalordered += im.ordered;
                    totalcancelled += im.cancelled_ship;
                    foreach(CancelItem ci in items)
                    {
                        if(ci.itemNumber == im.Sku)
                        {
                            totalcancelled += ci.quantity;
                        }
                    }
                }

                if(totalordered == totalcancelled)
                {
                    return master.shipping_amout;
                }
                else if(totalordered > totalcancelled)
                {
                    return 0;
                }

                return 0;
            }
        }
        public static async Task<JsonDocument> GetMagentoOrder(string token, string prefix, IConfiguration _configuration, string orderNumber)
        {

            JsonDocument response = JsonDocument.Parse("{}");

            using (var client = new HttpClient())
            {
                var uri = _configuration[prefix + ":baseurl"] + "orders?searchCriteria[filter_groups][0][filters][0][field]=increment_id&searchCriteria[filter_groups][0][filters][0][value]=" + orderNumber + "&searchCriteria[filter_groups][0][filters][0][condition_type]=eq";
                Console.WriteLine("OrderURL:" + uri.ToString());
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Uri u = new Uri(uri);
                HttpResponseMessage result = await client.GetAsync(u);
                if (result.IsSuccessStatusCode)
                {
                    response = JsonDocument.Parse(await result.Content.ReadAsStringAsync());
                    Console.Write("GetMagentoOrder Ok " + response.RootElement.ToString());
                }
                else
                {
                    Console.WriteLine("GetMagentoOrder Failed " + result.StatusCode.ToString());
                }
            }
            return response;
        }
        public static async Task<JsonDocument> GetMagentoOrderById(string token, string prefix, IConfiguration _configuration, int orderId)
        {

            JsonDocument response = JsonDocument.Parse("{}");

            using (var client = new HttpClient())
            {
                var uri = _configuration[prefix + ":baseurl"] + "orders/" + orderId;
                Console.WriteLine("OrderURL:" + uri.ToString());
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Uri u = new Uri(uri);
                HttpResponseMessage result = await client.GetAsync(u);
                if (result.IsSuccessStatusCode)
                {
                    response = JsonDocument.Parse(await result.Content.ReadAsStringAsync());
                    Console.Write("GetMagentoOrder Ok " + response.RootElement.ToString());
                }
                else
                {
                    Console.WriteLine("GetMagentoOrder Failed " + result.StatusCode.ToString());
                }
            }
            return response;
        }

        public static async Task<JsonDocument> GetMagentoCreditMemos(string token, string prefix, IConfiguration _configuration, string startDate, string endDate)
        {

            JsonDocument response = JsonDocument.Parse("{}");

            using (var client = new HttpClient())
            {
                var uri = _configuration[prefix + ":baseurl"] + "creditmemos?searchCriteria[filter_groups][0][filters][0][field]=created_at&searchCriteria[filter_groups][0][filters][0][value]=" + startDate + "&searchCriteria[filter_groups][0][filters][0][condition_type]=gt&searchCriteria[filter_groups][1][filters][1][field]=created_at&searchCriteria[filter_groups][1][filters][1][value]=" + endDate + "&searchCriteria[filter_groups][1][filters][1][condition_type]=lt";
                Console.WriteLine("CreditMemoURL:" + uri.ToString());
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Uri u = new Uri(uri);
                HttpResponseMessage result = await client.GetAsync(u);
                if (result.IsSuccessStatusCode)
                {
                    response = JsonDocument.Parse(await result.Content.ReadAsStringAsync());
                    Console.Write("GetMagentoCreditMemo Ok " + response.RootElement.ToString());
                }
                else
                {
                    Console.WriteLine("GetMagentoCreditMemo Failed " + result.StatusCode.ToString());
                }
            }
            return response;
        }

        public static async Task<JsonDocument> GetMagentoInvoice(string token, string prefix, IConfiguration _configuration, string orderId)
        {

            JsonDocument response = JsonDocument.Parse("{}");

            using (var client = new HttpClient())
            {
                var uri = _configuration[prefix + ":baseurl"] + "invoices?searchCriteria[filter_groups][0][filters][0][field]=order_id&searchCriteria[filter_groups][0][filters][0][value]=" + orderId + "&searchCriteria[filter_groups][0][filters][0][condition_type]=eq";
                Console.WriteLine("InvoiceURL:" + uri.ToString());
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Uri u = new Uri(uri);
                HttpResponseMessage result = await client.GetAsync(u);
                if (result.IsSuccessStatusCode)
                {
                    response = JsonDocument.Parse(await result.Content.ReadAsStringAsync());
                    Console.Write("GetMagentoInvoice Ok " + response.RootElement.ToString());
                }
                else
                {
                    Console.WriteLine("GetMagentoInvoice Failed " + result.StatusCode.ToString());
                }
            }
            return response;
        }

        public static async Task<JsonDocument> GetMagentoCreditMemo(string token, string prefix, IConfiguration _configuration, string creditMemoId)
        {


            JsonDocument response = JsonDocument.Parse("{}");
            using (var client = new HttpClient())
            {
                var uri = _configuration[prefix + ":baseurl"] + "creditmemos?searchCriteria[filter_groups][0][filters][0][field]=entity_id&searchCriteria[filter_groups][0][filters][0][value]=" + creditMemoId + "&searchCriteria[filter_groups][0][filters][0][condition_type]=eq";
                Console.WriteLine("CreditMemo:" + uri.ToString());
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Uri u = new Uri(uri);
                HttpResponseMessage result = await client.GetAsync(u);
                if (result.IsSuccessStatusCode)
                {
                    response = JsonDocument.Parse(await result.Content.ReadAsStringAsync());
                }
            }
            return response;

        }

        public static async Task<string> MagentoCreditMemoAsync(string token, string prefix, IConfiguration _configuration, JsonDocument invoiceJson, CancelOrder cancelOrder, string userName, string reason,double shipping_amount)
        {

            Console.WriteLine("invoiceJson:" + invoiceJson);

            JsonElement rootInvoice = invoiceJson.RootElement;
            JsonElement itemsElm = rootInvoice.GetProperty("items");

            if (itemsElm.GetArrayLength() == 0)
            {
                return "";
            }

            JsonElement itemElm = itemsElm[0];
            JsonElement invoiceEntityIdElm = itemElm.GetProperty("entity_id");
            JsonElement invoiceItemsElm = itemElm.GetProperty("items");

            var entityId = invoiceEntityIdElm.GetInt32();

            Console.WriteLine("invoiceId:" + entityId);

            List<int> returnList = new List<int>();
            List<Item> itemList = new List<Item>();
            Comment _comment = new Comment();
            _comment.comment = "User: " + userName + " Reason: " + reason + "  Item Number(s): ";
            string itemComments = "";

            double conditionAdjustment = 0;

            foreach (CancelItem cancelItem in cancelOrder.cancelItems)
            {

                Console.WriteLine("cancelItem:" + cancelItem.itemNumber);

                foreach (var row in invoiceItemsElm.EnumerateArray()
                        .Where(row => row.GetProperty("sku").GetString().Equals(cancelItem.itemNumber)))
                {


                    try
                    {
                        var rowTotal = row.GetProperty("row_total");
                        Item item = new Item();
                        item.order_item_id = row.GetProperty("order_item_id").GetInt32();
                        Console.WriteLine("order_item_id:" + row.GetProperty("order_item_id").GetInt32());
                        item.qty = cancelItem.quantity;
                        itemList.Add(item);
                        returnList.Add(row.GetProperty("order_item_id").GetInt32());
                        itemComments = itemComments + cancelItem.itemNumber + " Qty: " + cancelItem.quantity + " , ";
                        //if (cancelItem.reStockCode.Equals("used") && prefix.Equals("Invicta"))
                        //{
                        //    conditionAdjustment = Math.Round(conditionAdjustment + (cancelItem.quantity * row.GetProperty("price").GetDouble() * 0.2), 2); //20% fee
                        //}

                    }
                    catch
                    {
                        Console.WriteLine("skipping parent item");
                    }



                }
            }

            if (itemList.Count() == 0)
            {
                return "";
            }

            if (reason.Equals("RETURN") && prefix.Equals("TCO"))
            {
                itemComments = itemComments + " Return Shipping Fee: $7.50";
            }

            if (conditionAdjustment > 0 && prefix.Equals("Invicta"))
            {
                itemComments = itemComments + " Re-stocking Fee 20%: $" + conditionAdjustment;
            }

            _comment.comment = _comment.comment + itemComments;

            ExtensionAttributes extAtt = new ExtensionAttributes();
            extAtt.return_to_stock_items = returnList;

            Arguments arguments = new Arguments();
            arguments.extension_attributes = extAtt;
            if (reason.Equals("RETURN") && prefix.Equals("TCO"))
            {
                arguments.adjustment_negative = 7.50;
            }
            else
            {
                arguments.adjustment_negative = 0;
            }
            if (conditionAdjustment > 0 && prefix.Equals("Invicta"))
            {
                arguments.adjustment_negative = arguments.adjustment_negative + conditionAdjustment;
            }

            arguments.adjustment_positive = 0;
            arguments.shipping_amount = shipping_amount;

            Root root = new Root();
            root.appendComment = true;
            root.isOnline = true;
            root.comment = _comment;
            root.arguments = arguments;
            root.items = itemList;

            if (prefix.Equals("TCO"))
            {
                root.notify = false; //email customer  
            }
            else
            {
                root.notify = false; //Invicta Using Custom Email for now
            }

            string payload = JsonSerializer.Serialize(root);
            Console.WriteLine("payload: " + payload);
            HttpContent c = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");

            var uri = _configuration[prefix + ":baseurl"] + "invoice";
            uri = uri + "/" + entityId + "/refund";
            Uri u = new Uri(uri);

            var creditMemoResponse = await MagentoSupport.SendURI(u, c, token);
            Console.WriteLine("CreditMemo: " + creditMemoResponse.Replace("\"", ""));
            return creditMemoResponse.Replace("\"", "");
        }
    }
}
