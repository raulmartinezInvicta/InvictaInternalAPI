using InvictaInternalAPI.Context;
using InvictaInternalAPI.Exceptions;
using InvictaInternalAPI.Interfaces;
using InvictaInternalAPI.InvictaEntities;
using InvictaInternalAPI.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InvictaInternalAPI.Services
{
    public class CancelInvictaService : ICancelInvictaService
    {
        private readonly InvictaAUXContext _context;
        private readonly ILogger<CancelInvictaService> _logger;
        public CancelInvictaService(InvictaAUXContext context, ILogger<CancelInvictaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<ECommerceFulfillment> GetList(CancelOrder cancelOrder)
        {
            try
            {
                _logger.LogDebug("Extracting orders");
                var list = _context.ECommerceFulfillments.Where(x => x.OrderNo == cancelOrder.orderNumber).ToList();
                _logger.LogDebug($"Orders found: {list.Count}");
                return list;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception found: {e.Message}");
                throw new BusinessException($"Exception found: {e.Message}");
            }
           
        }

        public string GetItemLookupCode(int? orderEntryId)
        {
            try
            {
                _logger.LogDebug("Extracting itemlookup");
                var data = _context.ECommerceOrderEntries.FirstOrDefault(x => x.Id == orderEntryId);
                return data.ItemLookupCode;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception found: {e.Message}");
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }

        public int? GetId(int orderNumber)
        {
            try
            {
                _logger.LogDebug($"Extracting ID for the order: {orderNumber}");
                var data = _context.ECommerceOrders.FirstOrDefault(x => x.OrderNumber == orderNumber);
                return data.OrderId;

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
                _logger.LogDebug($"Extracting Order Lines for : {cancelOrder.orderNumber}");
                var list = _context.ECommerceOrderEntries.Where(x => x.OrderNumber == Convert.ToInt32(cancelOrder.orderNumber)).ToList();
                _logger.LogDebug($"Lines found: {list.Count}");
                return list;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception found: {e.Message}");
                throw new BusinessException($"Exception found: {e.Message}");
            }
            
        }

        public ECommerceOrderEntry GetOrdersInvicta(string order, string lookUp)
        {
            try
            {
                _logger.LogDebug("Extracting Invicta Orders");
                var data = _context.ECommerceOrderEntries.Where(x => x.ItemLookupCode.Contains(lookUp) && x.OrderNumber == Convert.ToInt32(order)).FirstOrDefault();
                _logger.LogDebug($"Order found : {data.OrderNumber}");
                return data;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception found: {e.Message}");
                throw new BusinessException($"Exception found: {e.Message}");
            }

        }
    }
}
