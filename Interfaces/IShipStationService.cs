using InvictaInternalAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvictaInternalAPI.Interfaces
{
    public interface IShipStationService
    {
        Task<bool> CancelShipStationOrder(CancelOrder cancelOrder);
    }
}
