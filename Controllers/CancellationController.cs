using ClosedXML.Excel;
using InvictaInternalAPI.Entities;
using InvictaInternalAPI.Exceptions;
using InvictaInternalAPI.Interfaces;
using InvictaInternalAPI.Model;
using InvictaInternalAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CancellationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ICancelService _cancel;
        private readonly ICancelInvictaService _cancelInvicta;
        private readonly IMagentoSupport _magento;
        private readonly ILogger<CancellationController> _logger;

        public CancellationController(IConfiguration configuration,ICancelService cancel, ILogger<CancellationController> logger, IMagentoSupport magento, ICancelInvictaService cancelInvicta)
        {
            _configuration = configuration;
            _cancel = cancel;
            _cancelInvicta = cancelInvicta;
            _magento = magento;
            _logger = logger;
        }
        
        [HttpGet("Excel")]
        public IActionResult GetExcel([FromQuery] DateTime? date, DateTime? date2, string company)
        {
            
            _logger.LogInformation("Running Excel/CancellationController endpoint execution");
            _logger.LogInformation($"Initial date: {date}");
            _logger.LogInformation($"Final date: {date2}");
            _logger.LogInformation($"Company: {company}");
            if (date > date2)
            {
                _logger.LogError("Inconsistency in date");
                throw new BusinessException("Inconsistency in date");
            }
            _logger.LogInformation("Obtaining information to export");
            var data = _cancel.GetExcelDB(date, date2, company);
            _logger.LogInformation("Information found successfully");
            if (data == null)
            {
                _logger.LogError("Data not available");
                throw new BusinessException("Data not available");
            }
            _logger.LogInformation("Creating xlsx file");
            using (MemoryStream stream = new MemoryStream())
            {
                var workbook = new XLWorkbook();
                var cont = 2;
                var worksheet = workbook.Worksheets.Add("Cancellation Orders");
                worksheet.Cell("A1").Value = "Date";
                worksheet.Cell("B1").Value = "Order";
                worksheet.Cell("C1").Value = "Partial/Complete";
                worksheet.Cell("D1").Value = "SKU";
                worksheet.Cell("E1").Value = "Reason";
                worksheet.Cell("F1").Value = "Comments";
                worksheet.Cell("G1").Value = "Payment Method";
                worksheet.Cell("H1").Value = "Amount Refunded";
                worksheet.Cell("I1").Value = "Site";
                foreach (Excel xl in data)
                {
                    worksheet.Cell($"A{cont}").Value = xl.Date;
                    worksheet.Cell($"B{cont}").Value = xl.OrderNumber;
                    worksheet.Cell($"C{cont}").Value = xl.Complete ? "Complete" : "Partial";
                    worksheet.Cell($"D{cont}").Value = xl.Sku;
                    worksheet.Cell($"E{cont}").Value = xl.Reason;
                    worksheet.Cell($"F{cont}").Value = xl.Quantity;
                    if(xl.Method.ToUpper() == "BRAINTREE")
                    {
                        worksheet.Cell($"G{cont}").Value = xl.CCType;
                    }
                    else
                    {
                        worksheet.Cell($"G{cont}").Value = xl.Method;

                    }
                    
                    worksheet.Cell($"H{cont}").Value = xl.Amount;
                    worksheet.Cell($"I{cont}").Value = xl.Prefix;
                    cont++;
                }

                workbook.SaveAs(stream);
                stream.Seek(0, SeekOrigin.Begin);
                _logger.LogInformation("Xlsx file ready to download");
                return this.File(
                    fileContents: stream.ToArray(),
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: "CancelOrders.xlsx"
                );
            }
        }

        [HttpGet("Orders")]
        public IActionResult GetCancelOrders([FromQuery]DateTime? date, DateTime? date2, string company)
        {
            _logger.LogInformation("Running endpoint : Orders/CancellationController");
            var data = _cancel.GetOrdersDB(date,date2,company);
            if(data == null)
            {
                throw new BusinessException("Data not available");
            }
            _logger.LogInformation("Process completed successfully");
            return Ok(data);
        }

        [HttpGet("Lines/Tco")]
        public IActionResult GetCancelLines([FromQuery]int cancelRequestID)
        {
            _logger.LogInformation("Running endpoint : Lines/Tco /CancellationController");
            var data = _cancel.GetLines(cancelRequestID);
            _logger.LogInformation("Process completed successfully");
            return Ok(data);
            
            
        }

        [HttpGet("Lines/Invicta")]
        public IActionResult GetCancelLinesInvicta([FromQuery] int cancelRequestID)
        {
            _logger.LogInformation("Running endpoint : Lines/Invicta /CancellationController");
            var data = _cancel.GetLinesInvicta(cancelRequestID);
            _logger.LogInformation("Process completed successfully");
            return Ok(data);
        }

        [HttpPost("Update")]
        public async Task<IActionResult> UpdateStep(StepUpdate stepUpdate)
        {
            _logger.LogInformation("Running endpoint : Update/CancellationController");
            var data = await _cancel.UpdateStep(stepUpdate);
            _logger.LogInformation("Process completed successfully");
            return Ok(data);
            
        }

        [HttpGet("Steps")]
        public IActionResult GetSteps([FromQuery] int cancelRequestID)
        {
            _logger.LogInformation("Running endpoint : Steps/CancellationController");
            var data =  _cancel.GetSteps(cancelRequestID);
            var list = new List<CancelRequestStepStatus>();
            foreach(CancelRequestStep s in data)
            {
                var item = new CancelRequestStepStatus()
                {
                        
                    Id = s.Id,
                    Link = s.Link,
                    Step = s.Step,
                    StatusStep = s.StatusStep ? "Done" : "Pending",
                    UpdatedBy = s.UpdatedBy,
                    UpdatedDate = s.UpdatedDate,
                    CancelRequestOrderId = s.CancelRequestOrderId
                };
                list.Add(item);
            }
            _logger.LogInformation("Process completed successfully");
            return Ok(list);
        }

        [HttpPost]
        public async Task<string> CancelSingleOrder(CancelOrder cancelOrder)
        {
            _logger.LogInformation("Running endpoint : CancelSingleOrder/CancellationController");
            if (cancelOrder.cancelItems == null)
            {
                _logger.LogError("You must add valid lines");
                throw new BusinessException("You must add valid lines");
            }
            var kind = true;
            var master = new MasterItem();
            var cancelRequestOrder = new CancelRequestOrder();
            
                if (cancelOrder.prefix.ToLower() == "tco")
                {
                _logger.LogInformation("Cancellation of TCO order");
                    var list = _cancel.GetList(cancelOrder);
                    if (list.Count != 0)
                    {
                        foreach (CancelItem i in cancelOrder.cancelItems)
                        {
                            var cont = new int();
                            var itemFullfilment = list.Where(x => x.ItemLookupCode.Contains(i.itemNumber)).ToList();
                            if(itemFullfilment.Count != 0)
                            {
                                var cd = itemFullfilment.Count();
                                if (i.quantity > itemFullfilment.Count())
                                {
                                    throw new BusinessException("Data inconsistent");
                                }

                                foreach(ECommerceFulfillment ef in itemFullfilment)
                                {
                                _logger.LogInformation("Preparing Merlin cancellation");
                                    var mglist = _cancel.GetOrderEntry(cancelOrder);
                                    var mgcount = mglist.Count();
                                    var itemOrderMagento = _cancel.GetOrdersTCO(cancelOrder.orderNumber, ef.ItemLookupCode);
                                    master = await ValidationQty(cancelOrder, mgcount, itemOrderMagento.ItemLookupCode);

                                    if (i.quantity > master.QtyAvailable)
                                    {
                                        _logger.LogError("Validation failed");
                                        _logger.LogError("Data inconsistent");
                                        throw new BusinessException("Data inconsistent");
                                    }

                                _logger.LogInformation("Successful validation");
                                    if (cont < i.quantity)
                                    {
                                    _logger.LogInformation($"Canceling order: {ef.OrderNo}");
                                        eCommerceActionSupport.eCommerceAction(ef.Id, 10, cancelOrder.prefix, _configuration);
                                        cont++;
                                    }

                                    if (cont == i.quantity)
                                    {
                                        i.itemNumber = itemOrderMagento.ItemLookupCode;
                                    }
                                }


                            }
                            else
                            {
                                var itemOrderEntry = _cancel.GetOrdersTCO(cancelOrder.orderNumber, i.itemNumber);
                                if (itemOrderEntry != null)
                                {
                                    kind = false;
                                _logger.LogInformation("Validating order");
                                var mglist = _cancel.GetOrderEntry(cancelOrder);
                                    var mgcount = mglist.Count();
                                    master = await ValidationQty(cancelOrder, mgcount, itemOrderEntry.ItemLookupCode);

                                    if (i.quantity > master.QtyAvailable)
                                    {
                                    _logger.LogError("Validation failed");
                                    _logger.LogError("Data inconsistent");
                                    throw new BusinessException("Data inconsistent");
                                    }
                                    
                                    i.itemNumber = itemOrderEntry.ItemLookupCode;
                                    
                                }

                            }

                        }

                        cancelRequestOrder =  CancellationProcess(cancelOrder);
                        
                    }
                    else
                    {
                        cancelRequestOrder =  CancellationProcess(cancelOrder);
                        kind = false;
                        foreach (CancelItem i in cancelOrder.cancelItems)
                        {
                            var it = _cancel.GetOrdersTCO(cancelOrder.orderNumber, i.itemNumber);
                            if (it != null)
                            {
                                i.itemNumber = it.ItemLookupCode;
                            }
                        _logger.LogInformation("Validating order");
                            var mglist = _cancel.GetOrderEntry(cancelOrder);
                            var mgcount = mglist.Count();
                            master = await ValidationQty(cancelOrder, mgcount, it.ItemLookupCode);

                            if (i.quantity > master.QtyAvailable)
                            {
                            _logger.LogError("Validation failed");
                            _logger.LogError("Data inconsistent");
                            throw new BusinessException("Data inconsistent");
                            }

                            i.itemNumber = it.ItemLookupCode;


                        }

                        
                    }
                }else if(cancelOrder.prefix.ToLower() == "invicta")
                {
                _logger.LogInformation("Cancellation of Invicta order");
                var list = _cancelInvicta.GetList(cancelOrder);
                    if (list != null)
                    {
                        foreach (CancelItem i in cancelOrder.cancelItems)
                        {
                            var cont = new int();
                            var e = _cancelInvicta.GetOrdersInvicta(cancelOrder.orderNumber, i.itemNumber);
                            foreach (InvictaEntities.ECommerceFulfillment ef in list)
                            {
                                if (i.itemNumber == ef.ItemLookupCode ||e.ItemLookupCode == ef.ItemLookupCode )
                                {
                                    var mglist = _cancelInvicta.GetOrderEntry(cancelOrder);
                                    var mgcount = mglist.Count();
                                _logger.LogInformation("Validating order");
                                master = await ValidationQty(cancelOrder, mgcount, e.ItemLookupCode);

                                    if (i.quantity > master.QtyAvailable)
                                    {
                                    _logger.LogError("Validation failed");
                                    _logger.LogError("Data inconsistent");
                                    throw new BusinessException("Data inconsistent");
                                    }
                                _logger.LogInformation("Successful validation");
                                if (cont < i.quantity)
                                    {
                                    _logger.LogInformation($"Canceling order: {ef.OrderNo}");
                                    eCommerceActionSupport.eCommerceAction(ef.Id, 10, cancelOrder.prefix, _configuration);
                                        cont++;
                                    }
                                    if (cont == i.quantity)
                                    {
                                        i.itemNumber = _cancelInvicta.GetItemLookupCode(ef.ECommerceOrderEntryId);
                                    }
                                }
                            }
                        }

                        cancelRequestOrder =  CancellationProcess(cancelOrder);
                    }
                    else
                    {
                    _logger.LogError("The order doesn't exist");
                        throw new BusinessException("The order doesn't exist");
                    }
                }else if (cancelOrder.prefix.ToLower() == "invictapartner")
                {
                _logger.LogInformation("Cancellation of InvictaPartner order");
                var list = _cancel.GetList(cancelOrder);
                    if (list.Count != 0)
                    {

                        foreach (CancelItem c in cancelOrder.cancelItems)
                        {
                            var cont = new int();
                            foreach (ECommerceFulfillment l in list)
                            {
                                if (c.itemNumber == l.ItemLookupCode)
                                {
                                    if(cont < c.quantity)
                                    {
                                    _logger.LogInformation($"Canceling order : {l.OrderNo}");
                                        eCommerceActionSupport.eCommerceAction(l.Id, 10, cancelOrder.prefix, _configuration);
                                        cont++;
                                    }
                                }
                            }

                        };


                        cancelRequestOrder =  CancellationProcess(cancelOrder);
                    _logger.LogInformation("Generating cancellation in ShipStation");
                        ShipStationCancelController ssCtrl = new ShipStationCancelController(_configuration, _cancel);
                        var shipStationResult = await ssCtrl.CancelShipStationOrder(cancelOrder);
                        var merlin = _cancel.UpdateStepGeneral(cancelRequestOrder, "Merlin");
                        var shipStation = _cancel.UpdateStepGeneral(merlin, "ShipStation");
                        await _cancel.AddOrder(shipStation);
                    _logger.LogInformation("Successful cancellation");
                        return "{\"creditmemo\": 0 }";
                    }
                    else
                    {
                        cancelRequestOrder =  CancellationProcess(cancelOrder);

                    //assuming the itemlookup is the same as the one entered
                    _logger.LogInformation("Generating cancellation in ShipStation");
                    ShipStationCancelController ssCtrl = new ShipStationCancelController(_configuration, _cancel);
                        var shipStationResult = await ssCtrl.CancelShipStationOrder(cancelOrder);
                        var merlin = _cancel.UpdateStepGeneral(cancelRequestOrder, "Merlin");
                        var shipStation = _cancel.UpdateStepGeneral(merlin, "ShipStation");
                        await _cancel.AddOrder(shipStation);
                    _logger.LogInformation("Successful cancellation");
                    return "{\"creditmemo\": 0 }";
                    }
                }
                
            

            try
            {
                _logger.LogInformation("Updating status of processed orders");
                var merlin =  _cancel.UpdateStepGeneral(cancelRequestOrder, "Merlin");
                _logger.LogInformation("Preparing magento cancellation");
                string response = await _magento.CancelOrder(cancelOrder.userName, cancelOrder.reason, cancelOrder.prefix, cancelOrder, _configuration,kind, merlin,master);

                

                if (string.IsNullOrEmpty(response))
                {
                    _logger.LogInformation("Magento cancellation failed");
                    _logger.LogInformation("Completed process");
                    return "{\"creditmemo\": 0 }";
                }
                else
                {
                    _logger.LogInformation("Completed process");
                    return "{\"creditmemo\": " + response + " }";
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception: {e.Message}");
                return "{\"creditmemo\": 0 }";
            }

        }


        private async Task<MasterItem> ValidationQty(CancelOrder cancelOrder,int count, string itemLookupMagento)
        {
            var magentoTokenJson = JObject.Parse(await MagentoSupport.GetMagentoToken(cancelOrder.prefix, _configuration));
            var token = magentoTokenJson["token"].ToString();
            int qty_available = 0;

            JsonDocument orderJson = await MagentoSupport.GetMagentoOrder(token, cancelOrder.prefix, _configuration, cancelOrder.orderNumber);

            JsonElement rootOrder = orderJson.RootElement;
            JsonElement itemsElm = rootOrder.GetProperty("items");

            bool flag = false;
            var ct = 0;
        
            while (flag != true  )
            {
                JsonElement items = itemsElm[0].GetProperty("items");
                JsonElement itemsku = items[ct].GetProperty("sku");
                if (itemsku.ToString() == itemLookupMagento)
                {
                    var qty_total = items[ct].GetProperty("qty_ordered").ToString();
                    var qty_canceled = items[ct].GetProperty("qty_canceled").ToString();
                    var qty_shipped = items[ct].GetProperty("qty_shipped").ToString();
                    var qty_refunded = items[ct].GetProperty("qty_refunded").ToString();
                    qty_available = Convert.ToInt32(qty_total) - Convert.ToInt32(qty_canceled) - Convert.ToInt32(qty_shipped) - Convert.ToInt32(qty_refunded);
                    flag = true;
                    if (qty_available <= 0)
                    {
                        throw new BusinessException("Item has no available quantity");
                    }
                }
                else
                {
                    ct++;
                }
            };

            var total_item_count = itemsElm[0].GetProperty("total_item_count").ToString();
            
            var cti = 0;
            bool flag2 = false;
            var itemsMagento = new List<ItemMagento>();
            JsonElement itemsM = itemsElm[0].GetProperty("items");
            var shipping_amount = itemsElm[0].GetProperty("shipping_amount").ToString();
            do
            {
                var qty_total = itemsM[cti].GetProperty("qty_ordered").ToString();
                var qty_canceled = itemsM[cti].GetProperty("qty_canceled").ToString();
                var qty_shipped = itemsM[cti].GetProperty("qty_shipped").ToString();
                var qty_refunded = itemsM[cti].GetProperty("qty_refunded").ToString();
                var sku = itemsM[cti].GetProperty("sku").ToString();

                if (Convert.ToInt32(qty_shipped) > 0)
                {
                    flag2 = true;
                }

                var im = new ItemMagento()
                {
                    Sku = sku,
                    ordered = Convert.ToInt32(qty_total),
                    cancelled_ship = Convert.ToInt32(qty_canceled)  + Convert.ToInt32(qty_refunded)
                };

                itemsMagento.Add(im);
                cti++;


            } while (cti< Convert.ToInt32(total_item_count));
            var master = new MasterItem()
            {
                shipping_amout = flag2 ? 0  : Convert.ToDouble(shipping_amount),
                QtyAvailable = qty_available,
                Items = itemsMagento
            };
            return master;

            
        }

        private CancelRequestOrder CancellationProcess(CancelOrder cancelOrder)
        {
            _logger.LogInformation($"Preparing information regarding order cancellation: {cancelOrder.orderNumber}");
                var cancelRequestOrder = new CancelRequestOrder()
                {
                    CreatedDate = DateTime.Now,
                    OrderNumber = Convert.ToInt32(cancelOrder.orderNumber),
                    Prefix = cancelOrder.prefix.ToUpper(),
                    Reason = cancelOrder.reason.ToUpper(),
                    UserName = cancelOrder.userName.ToUpper(),
                };

                foreach (CancelItem i in cancelOrder.cancelItems)
                {
                    var it = new CancelRequestItem()
                    {
                        Quantity = i.quantity,
                        Sku = i.itemNumber
                    };
                    cancelRequestOrder.CancelRequestItems.Add(it);
                    

                }

                var se = new CancelRequestStep()
                {
                    Link = "http://localhost:8080/ecom/viewer",
                    StatusStep = false,
                    Step = "Merlin",
                    UpdatedBy = cancelOrder.userName,
                    UpdatedDate = DateTime.Now,
                };
                cancelRequestOrder.CancelRequestSteps.Add(se);
                if (cancelOrder.prefix == "Invicta")
                {
                    var orderId = _cancelInvicta.GetId(Convert.ToInt32(cancelOrder.orderNumber));
                    var cancelRequestStep = new CancelRequestStep()
                    {

                        Link = $"{_configuration["Invicta:vieworder"]}{orderId}",
                        StatusStep = false,
                        Step = "Magento",
                        UpdatedBy = cancelOrder.userName,
                        UpdatedDate = DateTime.Now
                    };
                    cancelRequestOrder.CancelRequestSteps.Add(cancelRequestStep);


                }
                else
                {
                    var orderId = _cancel.GetId(cancelOrder.orderNumber);
                    var cancelRequestStep = new CancelRequestStep()
                    {
                        Link = $"{_configuration["TCO:vieworder"]}{orderId}",
                        StatusStep = false,
                        Step = "Magento",
                        UpdatedBy = cancelOrder.userName,
                        UpdatedDate = DateTime.Now
                    };
                    cancelRequestOrder.CancelRequestSteps.Add(cancelRequestStep);
                }

                var ss = new CancelRequestStep()
                {

                    Link = $"https://ship5.shipstation.com/shipments/advanced-search/order/all-orders-search-result?quickSearch={cancelOrder.orderNumber}",
                    StatusStep = false,
                    Step = "ShipStation",
                    UpdatedBy = cancelOrder.userName,
                    UpdatedDate = DateTime.Now
                };
                cancelRequestOrder.CancelRequestSteps.Add(ss);
            _logger.LogInformation("Cancel information saved in database successfully");
            return cancelRequestOrder;



        }

    }
}
