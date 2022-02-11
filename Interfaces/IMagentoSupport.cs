using InvictaInternalAPI.Entities;
using InvictaInternalAPI.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Interfaces
{
    public interface IMagentoSupport
    {
        Task<string> CancelOrder(String userName, String reason, String prefix, CancelOrder cancelOrder, IConfiguration _configuration, bool kind,CancelRequestOrder order, MasterItem master);
    }
}
