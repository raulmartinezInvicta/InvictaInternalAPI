using InvictaInternalAPI.Entities;
using InvictaInternalAPI.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Interfaces
{
    public interface ICancelService
    {
        List<Order> GetOrderJegSons();
        List<Order> GetOrderDesignerEyes();
        Task<bool> UpdateStep(StepUpdate stepUpdate);
        IEnumerable<ECommerceOrderEntry> GetOrderEntry(CancelOrder cancelOrder);
        int GetId(string orderNumber);
        IEnumerable<LinesItems> GetLinesInvicta(int? orderNo);
        IEnumerable<CancelRequestStep> GetSteps(int cancelRequestID);
        ECommerceOrderEntry GetOrdersTCO(string order, string lookUp);
        IEnumerable<LinesItems> GetLines(int? orderNo);
        Task AddOrder(CancelRequestOrder item);
        Task AddItems(List<CancelRequestItem> item);
        Task AddStep(CancelRequestStep step);
        List<ECommerceFulfillment> GetList(CancelOrder cancelOrder);
        CancelRequestOrder GetOrders(CancelOrder cancelOrder);
        CancelRequestOrder UpdateStepGeneral(CancelRequestOrder cancelRequestOrder, string context);
        IEnumerable<Excel> GetExcelDB(DateTime? date, DateTime? date2, string company);
        IEnumerable<CancelRequestOrderDTO> GetOrdersDB(DateTime? date, DateTime? date2, string company);
        Task<bool> UpdateCreditMemo(int orderNumber, string creditMemo);
    }
}
