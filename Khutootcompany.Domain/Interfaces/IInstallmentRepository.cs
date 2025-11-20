using Khutootcompany.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Domain.Interfaces
{
    public interface IInstallmentRepository : IRepository<Installment>
    {
        Task<IEnumerable<Installment>> GetActiveInstallmentsAsync();
        Task<IEnumerable<Installment>> GetOverdueInstallmentsAsync();
        Task<Installment?> GetInstallmentWithPaymentsAsync(int installmentId);
        Task<Installment?> GetByTruckIdAsync(int truckId);
        Task<IEnumerable<Installment>> GetByEmployeeIdAsync(int employeeId);
    }
}
