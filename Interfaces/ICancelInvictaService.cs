using InvictaInternalAPI.InvictaEntities;
using InvictaInternalAPI.Model;
using System.Collections.Generic;

namespace InvictaInternalAPI.Interfaces
{
    public interface ICancelInvictaService
    {
        List<ECommerceFulfillment> GetList(CancelOrder cancelOrder);
        string GetItemLookupCode(int? orderEntryId);
        IEnumerable<ECommerceOrderEntry> GetOrderEntry(CancelOrder cancelOrder);
        int? GetId(int orderNumber);
        ECommerceOrderEntry GetOrdersInvicta(string order, string lookUp);
    }
}
