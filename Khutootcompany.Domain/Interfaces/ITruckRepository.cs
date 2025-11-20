using Khutootcompany.Domain.Entities;
using Khutootcompany.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Domain.Interfaces
{
    public interface ITruckRepository : IRepository<Truck>
    {
        Task<IEnumerable<Truck>> GetTrucksWithCurrentDriverAsync();
        Task<IEnumerable<Truck>> GetTrucksWithExpiredInsuranceAsync();
        Task<IEnumerable<Truck>> GetTrucksWithExpiringSoonInsuranceAsync(int daysThreshold = 30);
        Task<IEnumerable<Truck>> GetTrucksByPAMStatusAsync(PAMStatus status);
        Task<IEnumerable<Truck>> GetTrucksByLicenseTypeAsync(LicenseType licenseType);
        Task<Truck?> GetTruckWithDetailsAsync(int truckId);
        Task<Truck?> GetByPlateNumberAsync(string plateNumber);
    }
}
