using InvictaInternalAPI.Interfaces;
using InvictaInternalAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipStationCancelController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ICancelService _cancel;

        public ShipStationCancelController(IConfiguration configuration,ICancelService cancel)
        {
            _configuration = configuration;
            _cancel = cancel;
        }

        [HttpPut]
        public async Task<ActionResult> CancelShipStationOrder(CancelOrder cancelOrder)
        {
            var username = _configuration["ShipStation:ssusername"];
            var password = _configuration["ShipStation:sspassword"];
            var urlget = _configuration["ShipStation:getOrderInfoUrl"];
            var urlpost = _configuration["ShipStation:postOrderInfoUrl"];
            int itemCount = cancelOrder.cancelItems.Count();
            decimal taxPercentage = 0;
            string jsonResponse = "";
            bool postSuccess = false;
            bool postSuccess2 = false;
            //bool splitPostSuccess = false;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            try
            {
                Uri u = new Uri(urlget + cancelOrder.orderNumber);
                int retryGet = 0;
                bool getSuccess = false;
                string result = "";
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add($"Authorization", $"Basic {Base64Encode($"{username}:{password}")}");
                while (retryGet <= 3 && !getSuccess)
                {
                    if (retryGet > 0) System.Threading.Thread.Sleep(20000);
                    var content = await httpClient.GetAsync(u);
                    if (content.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Get Ok: " + content.StatusCode.ToString());
                        getSuccess = true;
                        result = await content.Content.ReadAsStringAsync();
                        Console.WriteLine(result);
                    }
                    else
                    {
                        Console.WriteLine("Get Failed: " + content.StatusCode.ToString());
                    }
                    retryGet += 1;
                }
                if (getSuccess)
                {
                    SSRoot myDeserializedClass = JsonConvert.DeserializeObject<SSRoot>(result);


                    if (myDeserializedClass.orders.Count() == 0)
                    {
                        return StatusCode(StatusCodes.Status404NotFound);
                    }

                    List<SSItem> matchList = new List<SSItem>();
                    List<SSItem> misMatchList = new List<SSItem>();
                    foreach (SSOrder orderEntry in myDeserializedClass.orders)
                    {

                        if (orderEntry.orderStatus.Equals("shipped") || orderEntry.orderStatus.Equals("cancelled") || postSuccess)
                        {
                            continue;
                        }

                        int matchItemCount = 0;
                        int missMatchItemCount = 0;
                        decimal orderTotal = 0;
                        decimal splitOrderTotal = 0;
                        decimal taxAmount = 0;
                        decimal splitTaxAmount = 0;
                        int retryPost = 0;

                        if (orderEntry.taxAmount is null || orderEntry.orderTotal is null)
                        {
                            taxPercentage = 0;
                        }
                        else
                        {
                            try
                            {
                                taxPercentage = (decimal)(orderEntry.taxAmount / (orderEntry.orderTotal - orderEntry.taxAmount));
                            }
                            catch
                            {
                                taxPercentage = 0;
                            }

                        }

                        Console.WriteLine("orderItem[0]: " + orderEntry.items[0].sku + " " + orderEntry.items[0].quantity);

                        foreach (CancelItem pickItem in cancelOrder.cancelItems)
                        {
                            Console.WriteLine("pickItem: " + pickItem.itemNumber + " " + pickItem.quantity);

                            SSItem matchItem = orderEntry.items.Find(x => x.sku.Replace("AIC-", "").Equals(pickItem.itemNumber) && x.quantity == pickItem.quantity);
                            if (!(matchItem is null))
                            {
                                matchList.Add(matchItem);
                                matchItemCount += 1;
                                orderTotal = (decimal)(matchItem.unitPrice * matchItem.quantity);
                            }

                        }

                        foreach (SSItem item in orderEntry.items)
                        {

                            CancelItem matchItem = cancelOrder.cancelItems.Find(x => x.itemNumber.Equals(item.sku.Replace("AIC-", "")) && x.quantity == item.quantity);
                            if (matchItem is null && item.unitPrice >= 0)
                            {
                                misMatchList.Add(item);
                                missMatchItemCount += 1;
                                splitOrderTotal = (decimal)(item.unitPrice * item.quantity);
                            }

                        }

                        Console.WriteLine("itemCount:" + itemCount + " matchItemCount:" + matchItemCount + " missMatchItemCount:" + missMatchItemCount);

                        if (matchItemCount == itemCount && missMatchItemCount == 0)
                        {
                            //cancel old order
                            orderEntry.orderStatus = "cancelled";
                            jsonResponse = System.Text.Json.JsonSerializer.Serialize(orderEntry, options);
                            while (retryPost <= 3 && !postSuccess)
                            {
                                if (retryPost > 0) System.Threading.Thread.Sleep(2000);
                                Uri uPost = new Uri(urlpost);
                                HttpContent cPost = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json");
                                var contentPost = await httpClient.PostAsync(uPost, cPost);
                                if (contentPost.IsSuccessStatusCode)
                                {
                                    Console.WriteLine("Post Ok: " + contentPost.StatusCode.ToString());
                                    postSuccess = true;
                                }
                                else
                                {
                                    Console.WriteLine("Post Failed: " + contentPost.StatusCode.ToString());

                                }
                                retryPost += 1;
                            }
                        }
                        //else
                        //{
                        //    return StatusCode(StatusCodes.Status404NotFound);
                        //}


                        if (matchItemCount == itemCount && missMatchItemCount > 0)
                        {   // some missmatch
                            //remove cancelled items from order
                            Console.WriteLine("SS Order Update");
                            //orderEntry.orderStatus = "awaiting_shipment";  //order is already in the status
                            taxAmount = orderTotal * taxPercentage;
                            orderTotal = orderTotal + taxAmount;
                            splitTaxAmount = splitOrderTotal * taxPercentage;
                            splitOrderTotal = splitOrderTotal + splitTaxAmount;
                            Uri uPost = new Uri(urlpost);
                            //orderEntry.orderId = 0;  //updating existing order
                            //orderEntry.orderKey = null;  //updating existing order
                            orderEntry.items = misMatchList;
                            orderEntry.orderTotal = splitOrderTotal;
                            orderEntry.amountPaid = splitOrderTotal;
                            orderEntry.taxAmount = splitTaxAmount;
                            orderEntry.shippingAmount = 0;
                            var splitOrderJson = System.Text.Json.JsonSerializer.Serialize(orderEntry, options);
                            int retryUpdate = 0;
                            while (retryUpdate <= 3 && !postSuccess2)
                            {
                                if (retryUpdate > 0) System.Threading.Thread.Sleep(2000);
                                HttpContent cSplitPost = new StringContent(splitOrderJson, System.Text.Encoding.UTF8, "application/json");
                                var splitContentPost = await httpClient.PostAsync(uPost, cSplitPost);
                                if (splitContentPost.IsSuccessStatusCode)
                                {
                                    Console.WriteLine("Update Post Ok: " + splitContentPost.StatusCode.ToString());
                                    postSuccess2 = true;
                                }
                                else
                                {
                                    Console.WriteLine("Update Post Failed: " + splitContentPost.StatusCode.ToString());
                                }
                                retryUpdate += 1;

                            }

                            Console.WriteLine("SS Order Cancelled");
                            orderEntry.orderStatus = "cancelled";
                            Uri uPost2 = new Uri(urlpost);
                            orderEntry.orderId = 0;  //creating order
                            orderEntry.orderKey = null;  //creating order
                            orderEntry.items = matchList;
                            orderEntry.orderTotal = orderTotal;
                            orderEntry.amountPaid = orderTotal;
                            orderEntry.taxAmount = taxAmount;
                            orderEntry.shippingAmount = 0;
                            var createOrderJson2 = System.Text.Json.JsonSerializer.Serialize(orderEntry, options);
                            int retryUpdate2 = 0;
                            while (retryUpdate2 <= 3 && !postSuccess)
                            {
                                if (retryUpdate2 > 0) System.Threading.Thread.Sleep(20000);
                                HttpContent cCreatePost = new StringContent(createOrderJson2, System.Text.Encoding.UTF8, "application/json");
                                var createPost = await httpClient.PostAsync(uPost, cCreatePost);
                                if (createPost.IsSuccessStatusCode)
                                {
                                    Console.WriteLine("Create Post Ok: " + createPost.StatusCode.ToString());
                                    postSuccess = true;
                                }
                                else
                                {
                                    Console.WriteLine("Create Post Failed: " + createPost.StatusCode.ToString());
                                }
                                retryUpdate2 += 1;
                            }
                        }

                        
                    }
                }
                else
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }

                if (postSuccess)
                {
                    return StatusCode(StatusCodes.Status200OK);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }


        }

        [NonAction]
        public static string Base64Encode(string textToEncode)
        {
            byte[] textAsBytes = Encoding.UTF8.GetBytes(textToEncode);
            return Convert.ToBase64String(textAsBytes);
        }


    }
}
