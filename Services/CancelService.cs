using InvictaInternalAPI.Context;
using InvictaInternalAPI.Entities;
using InvictaInternalAPI.Exceptions;
using InvictaInternalAPI.Interfaces;
using InvictaInternalAPI.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Services
{
    public class CancelService : ICancelService
    {
        private readonly MerlinContext _context;
        private readonly InvictaAUXContext _invContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CancelService> _logger;

        public CancelService(MerlinContext context, ILogger<CancelService> logger, IConfiguration configuration, InvictaAUXContext invictaAUXContext)
        {
            _context = context;
            _configuration = configuration;
            _invContext = invictaAUXContext;
            _logger = logger;
        }

        public List<Order> GetOrderJegSons()
        {
            try
            {
                var Connection = _configuration.GetConnectionString("DefaultConnectionMerlin");
                _logger.LogDebug("Extracting orders from Merlin");
                using (SqlConnection connection = new SqlConnection(Connection))
                {
                    SqlDataAdapter da = new SqlDataAdapter();
                    DataSet ds = new DataSet();
                    SqlCommand command = new SqlCommand("dbo.Invicta_SP_Qry_JegSons", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    da = new SqlDataAdapter(command);
                    da.Fill(ds);
                    var dat = ds.Tables[0];
                    List<Order> list = new List<Order>();
                    list = (from DataRow dr in dat.Rows
                            select new Order()
                            {
                                ContactEmail = getString(dr, "ContactEmail"),
                                EntryID = getString(dr, "EntryID"),
                                FirstName = getString(dr, "FirstName"),
                                LastName = getString(dr, "LastName"),
                                ID = getString(dr, "ID"),
                                isPendingForwarding = getString(dr, "isPendingForwarding"),
                                City = getString(dr, "City"),
                                ItemLookupCode = getString(dr, "ItemLookupCode"),
                                OrderNumber = getString(dr, "OrderNumber"),
                                Country = getString(dr, "Country"),
                                PostCode = getString(dr, "PostCode"),
                                QtyCancelled = getString(dr, "QtyCancelled"),
                                QtyOrdered = getString(dr, "QtyOrdered"),
                                QtyRefunded = getString(dr, "QtyRefunded"),
                                QtyShipped = getString(dr, "QtyShipped"),
                                RealQty = getString(dr, "RealQty"),
                                Region = getString(dr, "Region"),
                                RegionID = getString(dr, "RegionID"),
                                SimpleProdLineNo = getString(dr, "SimpleProdLineNo"),
                                Status = getString(dr, "Status"),
                                Street = getString(dr, "Street"),
                                Street2 = getString(dr, "Street2"),
                                Telephone = getString(dr, "Telephone"),
                                wasForwarded = getString(dr, "wasForwarded")
                            }).ToList();
                    _logger.LogDebug("Connection with SQL DB succesful");
                    _logger.LogDebug("Extraction succesfully");
                    return list;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception found: {e.Message}");
                throw new BusinessException($"Exception found: {e.Message}");
            }

        }

        public List<Order> GetOrderDesignerEyes()
        {
            try
            {
                var Connection = _configuration.GetConnectionString("DefaultConnectionMerlin");
                _logger.LogDebug("Extracting orders from Merlin");
                using (SqlConnection connection = new SqlConnection(Connection))
                {
                    SqlDataAdapter da = new SqlDataAdapter();
                    DataSet ds = new DataSet();
                    SqlCommand command = new SqlCommand("dbo.Invicta_SP_Qry_DesignerEyes", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    da = new SqlDataAdapter(command);
                    da.Fill(ds);
                    var dat = ds.Tables[0];
                    List<Order> list = new List<Order>();
                    list = (from DataRow dr in dat.Rows
                            select new Order()
                            {
                                ContactEmail = getString(dr, "ContactEmail"),
                                EntryID = getString(dr, "EntryID"),
                                FirstName = getString(dr, "FirstName"),
                                LastName = getString(dr, "LastName"),
                                ID = getString(dr, "ID"),
                                isPendingForwarding = getString(dr, "isPendingForwarding"),
                                City = getString(dr, "City"),
                                ItemLookupCode = getString(dr, "ItemLookupCode"),
                                OrderNumber = getString(dr, "OrderNumber"),
                                Country = getString(dr, "Country"),
                                PostCode = getString(dr, "PostCode"),
                                QtyCancelled = getString(dr, "QtyCancelled"),
                                QtyOrdered = getString(dr, "QtyOrdered"),
                                QtyRefunded = getString(dr, "QtyRefunded"),
                                QtyShipped = getString(dr, "QtyShipped"),
                                RealQty = getString(dr, "RealQty"),
                                Region = getString(dr, "Region"),
                                RegionID = getString(dr, "RegionID"),
                                SimpleProdLineNo = getString(dr, "SimpleProdLineNo"),
                                Status = getString(dr, "Status"),
                                Street = getString(dr, "Street"),
                                Street2 = getString(dr, "Street2"),
                                Telephone = getString(dr, "Telephone"),
                                wasForwarded = getString(dr, "wasForwarded")
                            }).ToList();
                    _logger.LogDebug("Connection with SQL DB succesful");
                    _logger.LogDebug("Extraction succesfully");
                    return list;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception found: {e.Message}");
                throw new BusinessException($"Exception found: {e.Message}");
            }

        }
        public  IEnumerable<CancelRequestStep> GetSteps(int cancelRequestID)
        {
            try
            {
                _logger.LogDebug("Getting steps for the database");
                var steps = _context.CancelRequestSteps.Where(x => x.CancelRequestOrderId == cancelRequestID).ToList();
                _logger.LogDebug($"Information found: {steps.Count}");
                return steps;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception found: {e.Message}");
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }

        public async Task<bool> UpdateCreditMemo(int orderNumber,string creditMemo)
        {
            try
            {
                _logger.LogDebug($"Updating CreditMemo {creditMemo}");
                var data = _context.CancelRequestOrders.FirstOrDefault(x => x.OrderNumber == orderNumber);
                data.CreditMemo = creditMemo;
                _context.CancelRequestOrders.Update(data);
                var response = await _context.SaveChangesAsync();
                _logger.LogDebug($"Response obtained: {response}");
                return response > 0;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception found: {e.Message}");
                throw new BusinessException($"Exception found: {e.Message}");
            }
            

        }

        public IEnumerable<LinesItems> GetLines(int? cancelRequestID)
        {
            try
            {
                _logger.LogDebug("Generating connection with DB");
                var Connection = _configuration.GetConnectionString("DefaultConnectionMerlin");
                _logger.LogDebug($"ConnectionString : {Connection}");
                SqlConnection connection = new SqlConnection(Connection);
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                SqlCommand command = new SqlCommand("dbo.Invicta_SP_Qry_InternalAPI", connection);
                command.CommandType = CommandType.StoredProcedure;
                if (cancelRequestID != null)
                {
                    command.Parameters.AddWithValue("@CustomerNo", cancelRequestID);
                }
                da = new SqlDataAdapter(command);
                da.Fill(ds);
                var dat = ds.Tables[0];
                List<LinesItems> list = new List<LinesItems>();
                list = (from DataRow dr in dat.Rows
                        select new LinesItems()
                        {
                            UserName = getString(dr, "userName"),
                            Date = getDateTime(dr, "createdDate"),
                            Name = getString(dr, "Name"),
                            Sku = getString(dr, "sku"),
                            OrderNumber = getInt(dr, "orderNumber"),
                            Prefix = getString(dr, "prefix"),
                            Price = getDecimal(dr, "Price"),
                            Quantity = getInt(dr, "quantity"),
                            RowTotalInclTax = getString(dr, "RowTotalInclTax"),
                            Method = getString(dr, "Method") + " " + getString(dr, "CCType")
                        }).ToList();
                _logger.LogDebug("Data extraction completed");
                return list;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception found: {e.Message}");
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }

        public IEnumerable<LinesItems> GetLinesInvicta(int? cancelRequestID)
        {
            try
            {
                _logger.LogDebug("Generating connection with DB");
                var Connection = _configuration.GetConnectionString("DefaultConnectionMerlin");
                _logger.LogDebug($"ConnectionString : {Connection}");
                SqlConnection connection = new SqlConnection(Connection);
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                SqlCommand command = new SqlCommand("dbo.Invicta_SP_Qry_InternalAPI_Invicta", connection);
                command.CommandType = CommandType.StoredProcedure;
                if (cancelRequestID != null)
                {
                    command.Parameters.AddWithValue("@CustomerNo", cancelRequestID);
                }
                da = new SqlDataAdapter(command);
                da.Fill(ds);
                var dat = ds.Tables[0];
                List<LinesItems> list = new List<LinesItems>();
                list = (from DataRow dr in dat.Rows
                        select new LinesItems()
                        {
                            UserName = getString(dr, "userName"),
                            Date = getDateTime(dr, "createdDate"),
                            Name = getString(dr, "Name"),
                            Sku = getString(dr, "sku"),
                            OrderNumber = getInt(dr, "orderNumber"),
                            Prefix = getString(dr, "prefix"),
                            Price = getDecimal(dr, "Price"),
                            Quantity = getInt(dr, "quantity"),
                            RowTotalInclTax = getString(dr, "RowTotalInclTax"),
                            Method = getString(dr, "Method") + " " + getString(dr, "CCType")
                        }).ToList();
                _logger.LogDebug("Data extraction completed");
                return list;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception found: {e.Message}");
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }

        public IEnumerable<Excel> GetExcelDB(DateTime? date, DateTime? date2, string company)
        {
            try
            {
                string store;
                _logger.LogDebug("Preparing SP to DB query");
                if (company.ToUpper() == "TCO")
                {
                    store = "dbo.Orders_excel_Merlin";
                }
                else
                {
                    store = "dbo.Orders_excel_Invicta";
                }
                _logger.LogDebug($"SP: {store}");
                var Connection = _configuration.GetConnectionString("DefaultConnectionMerlin");
                _logger.LogDebug($"ConnectionString : {Connection}");
                SqlConnection connection = new SqlConnection(Connection);
                SqlDataAdapter da = new SqlDataAdapter();
                DataSet ds = new DataSet();
                SqlCommand command = new SqlCommand(store, connection);
                command.CommandType = CommandType.StoredProcedure;
                if (date != null)
                {
                    command.Parameters.AddWithValue("@Start", date);
                    command.Parameters.AddWithValue("@End", date2);
                }
                da = new SqlDataAdapter(command);
                da.Fill(ds);
                var dat = ds.Tables[0];
                List<Excel> list = new List<Excel>();
                list = (from DataRow dr in dat.Rows
                        select new Excel()
                        {
                            Amount = getString(dr, "RowTotalInclTax"),
                            Date = getDateTime(dr, "createdDate"),
                            CCType = getString(dr, "CCType"),
                            Sku = getString(dr, "sku"),
                            OrderNumber = getString(dr, "orderNumber"),
                            Prefix = getString(dr, "prefix"),
                            Reason = getString(dr, "reason"),
                            Quantity = getString(dr, "quantity"),
                            Method = getString(dr, "Method"),
                            Complete = true
                        }).ToList();
                _logger.LogDebug("Data extraction completed");

                foreach (Excel xl in list)
                {
                    if (company.ToUpper() == "INVICTA")
                    {
                        var di = _invContext.ECommerceFulfillments.Where(x => x.OrderNo == Convert.ToString(xl.OrderNumber)).ToList();
                        foreach (InvictaEntities.ECommerceFulfillment ie in di)
                        {
                            if (ie.Status != 10)
                            {
                                xl.Complete = false;
                            }
                        }
                    }
                    else
                    {
                        var dt = _context.ECommerceFulfillments.Where(x => x.OrderNo == Convert.ToString(xl.OrderNumber)).ToList();
                        foreach (ECommerceFulfillment it in dt)
                        {
                            if (it.Status != 10)
                            {
                                xl.Complete = false;
                            }
                        }
                    }
                }
                return list;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception found: {e.Message}");
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }

        public  IEnumerable<CancelRequestOrderDTO> GetOrdersDB(DateTime? date,DateTime? date2, string company)
        {
            try
            {
                _logger.LogDebug("Getting canceled orders from the db");
                var data = _context.CancelRequestOrders.AsEnumerable();
                if (date != null)
                {
                    _logger.LogDebug("Filtering by date");
                    data = data.Where(e => e.CreatedDate.Date >= date && e.CreatedDate.Date <= date2).ToList();
                }
                if (company != null)
                {
                    data = data.Where(e => e.Prefix == company.ToUpper()).ToList();
                    _logger.LogDebug("Filtering by company");
                }

                _logger.LogDebug("Generating final order list");
                var orders = new List<CancelRequestOrderDTO>();
                foreach (CancelRequestOrder co in data)
                {

                    var steps = GetSteps(co.Id);
                    var order = new CancelRequestOrderDTO()
                    {
                        CreatedDate = co.CreatedDate,
                        CreditMemo = co.CreditMemo,
                        Id = co.Id,
                        OrderNumber = co.OrderNumber,
                        Prefix = co.Prefix,
                        Reason = co.Reason,
                        UserName = co.UserName,
                        Status = true,
                        Complete = true
                    };
                    foreach (CancelRequestStep cs in steps)
                    {
                        if (cs.StatusStep == false)
                        {
                            order.Status = false;
                        }
                    }

                    if (co.Prefix.ToUpper() == "INVICTA")
                    {
                        var di = _invContext.ECommerceFulfillments.Where(x => x.OrderNo == Convert.ToString(co.OrderNumber)).ToList();
                        foreach (InvictaEntities.ECommerceFulfillment ie in di)
                        {
                            if (ie.Status != 10)
                            {
                                order.Complete = false;
                            }
                        }
                    }
                    else
                    {
                        var dt = _context.ECommerceFulfillments.Where(x => x.OrderNo == Convert.ToString(co.OrderNumber)).ToList();
                        foreach (ECommerceFulfillment it in dt)
                        {
                            if (it.Status != 10)
                            {
                                order.Complete = false;
                            }
                        }
                    }

                    orders.Add(order);
                }
                _logger.LogDebug($"Total Orders: {orders.Count}");
                return orders;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception found: {e.Message}");
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }

        public ECommerceOrderEntry GetOrdersTCO(string order ,string lookUp)
        {
            try
            {
                _logger.LogDebug("Obtaining TCO orders");
                var data = _context.ECommerceOrderEntries.Where(x => x.ItemLookupCode.Contains(lookUp) && x.OrderNumber == order).FirstOrDefault();
                if (data != null)
                {
                    _logger.LogDebug($"Order found successfully");
                }

                return data;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception found: {e.Message}");
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }

        public int GetId(string orderNumber)
        {
            try
            {
                _logger.LogDebug("Getting Order ID");
                var data = _context.ECommerceOrders.FirstOrDefault(x => x.OrderNumber == orderNumber);
                _logger.LogDebug($"ID Obtained: {data.OrderId}");
                return data.OrderId;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception found: {e.Message}");
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }

        public async Task<bool> UpdateStep(StepUpdate stepUpdate)
        {
            try
            {
                _logger.LogDebug($"Updating step : {stepUpdate.step} for Order ID : {stepUpdate.cancelRequestID}");
                var steps = _context.CancelRequestSteps.Where(x => x.CancelRequestOrderId == stepUpdate.cancelRequestID).ToList();
                foreach (CancelRequestStep cs in steps)
                {
                    if (cs.Step == stepUpdate.step)
                    {
                        cs.StatusStep = stepUpdate.statusStep;
                        cs.UpdatedDate = DateTime.Now;
                        cs.UpdatedBy = stepUpdate.updatedBy;
                        _context.CancelRequestSteps.Update(cs);
                        var response = await _context.SaveChangesAsync();
                        _logger.LogDebug($"Final Status Update : {response}");
                        return response > 0;
                    }
                }
                _logger.LogDebug("Failed update");
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception found: {e.Message}");
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
            
        }
        public CancelRequestOrder UpdateStepGeneral(CancelRequestOrder cancelRequestOrder,string context)
        {
            try
            {
                _logger.LogDebug($"Updating general status for the order : {cancelRequestOrder.OrderNumber}");
                var merlin = cancelRequestOrder.CancelRequestSteps.FirstOrDefault(x => x.Step == context);
                merlin.StatusStep = true;
                _logger.LogDebug("Updated status");
                return cancelRequestOrder;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception found: {e.Message}");
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
            
        }

        
        public CancelRequestOrder GetOrders(CancelOrder cancelOrder)
        {
            try
            {
                _logger.LogDebug($"Getting order : {cancelOrder.orderNumber}");
                var list = _context.CancelRequestOrders.FirstOrDefault(x => x.OrderNumber == Convert.ToInt32(cancelOrder.orderNumber));
                _logger.LogDebug($"Order obtained : {list.OrderNumber}");
                return list;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception found: {e.Message}");
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }

        public IEnumerable<ECommerceOrderEntry> GetOrderEntry(CancelOrder cancelOrder)
        {
            try
            {
                _logger.LogDebug($"Getting lines from the order: {cancelOrder.orderNumber}");
                var list = _context.ECommerceOrderEntries.Where(x => x.OrderNumber == cancelOrder.orderNumber).ToList();
                _logger.LogDebug($"Lines found: {list.Count}");
                return list;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception found: {e.Message}");
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }


        public List<ECommerceFulfillment> GetList(CancelOrder cancelOrder)
        {
            try
            {
                _logger.LogDebug($"Extracting fullfilments for the order: {cancelOrder.orderNumber}");
                var list = _context.ECommerceFulfillments.Where(x => x.OrderNo == cancelOrder.orderNumber).ToList();
                _logger.LogDebug($"Fullfilments found: {list.Count}");
                return list;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception found: {e.Message}");
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }
        public async Task AddOrder(CancelRequestOrder item)
        {
            try
            {
                _logger.LogDebug($"Adding order to cancel: {item.OrderNumber}");
                _context.CancelRequestOrders.Add(item);
                await _context.SaveChangesAsync();

            }
            catch (Exception e)
            {
                _logger.LogError($"Exception found: {e.Message}");
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }

        public async Task AddItems(List<CancelRequestItem> item)
        {
            try
            {
                _logger.LogDebug($"Adding items to cancel: {item.Count}");
                _context.CancelRequestItems.AddRange(item);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception found: {e.Message}");
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }

        public async Task AddStep(CancelRequestStep step)
        {
            try
            {
                _logger.LogDebug($"Adding steps to cancel: {step.Step}");
                _context.CancelRequestSteps.Add(step);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception found: {e.Message}");
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }

        #region Gets

        private decimal getDecimal(DataRow data, string param)
        {
            if (data[$"{param}"] == DBNull.Value)
            {
                return 0;
            }
            else
            {
                return Convert.ToDecimal(data[$"{param}"]);
            }
        }

        private string getCurrency(DataRow data, string param)
        {
            if (data[$"{param}"] == DBNull.Value)
            {
                return "USD";
            }
            else
            {
                return Convert.ToString(data[$"{param}"]);
            }
        }

        private long getLong(DataRow data, string param)
        {
            if (data[$"{param}"] == DBNull.Value)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(data[$"{param}"]);
            }
        }

        private int getInt(DataRow data, string param)
        {
            if (data[$"{param}"] == DBNull.Value)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(data[$"{param}"]);
            }
        }

        private DateTime getDateTime(DataRow data, string param)
        {
            if (data[$"{param}"] == DBNull.Value)
            {
                return DateTime.UtcNow;
            }
            else
            {
                return Convert.ToDateTime(data[$"{param}"]);
            }
        }

        private string getString(DataRow data, string param)
        {
            if (data[$"{param}"] == DBNull.Value)
            {
                return "Empty";
            }
            else
            {
                return Convert.ToString(data[$"{param}"]);
            }
        }
        #endregion
    }
}
