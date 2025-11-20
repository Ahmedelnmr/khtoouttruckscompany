using Khutootcompany.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Domain.Interfaces
{
    public interface IWakalaRepository : IRepository<Wakala>
    {
        Task<IEnumerable<Wakala>> GetExpiredWakalatAsync();
        Task<IEnumerable<Wakala>> GetExpiringSoonWakalatAsync(int daysThreshold = 30);
        Task<IEnumerable<Wakala>> GetGeneralWakalatAsync();
        Task<IEnumerable<Wakala>> GetWakalatByEmployeeAsync(int employeeId);
        Task<IEnumerable<Wakala>> GetWakalatByTruckAsync(int truckId);
    }
}
