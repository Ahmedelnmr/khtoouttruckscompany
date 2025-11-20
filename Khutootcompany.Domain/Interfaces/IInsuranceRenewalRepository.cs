using Khutootcompany.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Domain.Interfaces
{
    public interface IInsuranceRenewalRepository : IRepository<InsuranceRenewal>
    {
        Task<IEnumerable<InsuranceRenewal>> GetRenewalsByTruckAsync(int truckId);
        Task<InsuranceRenewal?> GetLatestRenewalForTruckAsync(int truckId);
    }
}
