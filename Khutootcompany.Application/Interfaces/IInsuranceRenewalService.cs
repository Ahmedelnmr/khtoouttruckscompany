using Khutootcompany.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.Interfaces
{
    public interface IInsuranceRenewalService
    {
        Task RenewInsuranceAsync(int truckId, DateTime expiryDate, decimal cost, string? sondNumber, string username);
        Task<IEnumerable<InsuranceRenewal>> GetRenewalsByTruckAsync(int truckId);
    }
}
