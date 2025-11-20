using Khutootcompany.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.Interfaces
{
    public interface IWakalaService
    {
        Task<IEnumerable<WakalaDto>> GetAllWakalatAsync();
        Task<WakalaDto?> GetWakalaByIdAsync(int id);
        Task<IEnumerable<WakalaDto>> GetExpiredWakalatAsync();
        Task<IEnumerable<WakalaDto>> GetExpiringSoonWakalatAsync(int days = 30);
        Task<IEnumerable<WakalaDto>> GetGeneralWakalatAsync();
        Task<IEnumerable<WakalaDto>> GetWakalatByEmployeeAsync(int employeeId);
        Task<IEnumerable<WakalaDto>> GetWakalatByTruckAsync(int truckId);
        Task<WakalaDto> CreateWakalaAsync(WakalaDto dto, string username);
        Task<WakalaDto> UpdateWakalaAsync(WakalaDto dto, string username);
        Task DeleteWakalaAsync(int id, string username);
    }
}
