using Khutootcompany.Application.DTOs;
using Khutootcompany.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.Interfaces
{
    public interface ITruckService
    {
        Task<IEnumerable<TruckDto>> GetAllTrucksAsync();
        Task<TruckDto?> GetTruckByIdAsync(int id);
        Task<TruckDto?> GetTruckByPlateAsync(string plate);
        Task<IEnumerable<TruckDto>> GetTrucksWithExpiredInsuranceAsync();
        Task<IEnumerable<TruckDto>> GetTrucksWithExpiringSoonInsuranceAsync(int days = 30);
        Task<IEnumerable<TruckDto>> GetTrucksByPAMStatusAsync(PAMStatus status);
        Task<IEnumerable<TruckDto>> GetTrucksByLicenseTypeAsync(LicenseType licenseType);
        Task<TruckDto> CreateTruckAsync(TruckDto dto, string username);
        Task<TruckDto> UpdateTruckAsync(TruckDto dto, string username);
        Task DeleteTruckAsync(int id, string username);
        Task AssignDriverAsync(int truckId, int employeeId, string username);
    }
}
