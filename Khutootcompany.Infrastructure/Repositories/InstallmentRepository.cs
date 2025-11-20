using Khutootcompany.Domain.Entities;
using Khutootcompany.Domain.Interfaces;
using Khutootcompany.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Infrastructure.Repositories
{
    public class InstallmentRepository : GenericRepository<Installment>, IInstallmentRepository
    {
        public InstallmentRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Installment>> GetActiveInstallmentsAsync()
        {
            return await _dbSet
                .Where(i => !i.IsCompleted)
                .Include(i => i.Truck)
                .Include(i => i.Employee)
                .Include(i => i.Payments)
                .ToListAsync();
        }

        public async Task<IEnumerable<Installment>> GetOverdueInstallmentsAsync()
        {
            var installments = await GetActiveInstallmentsAsync();
            return installments.Where(i => i.IsOverdue());
        }

        public async Task<Installment?> GetInstallmentWithPaymentsAsync(int installmentId)
        {
            return await _dbSet
                .Include(i => i.Truck)
                .Include(i => i.Employee)
                .Include(i => i.Payments.OrderByDescending(p => p.PaymentDate))
                .FirstOrDefaultAsync(i => i.InstallmentId == installmentId);
        }

        public async Task<Installment?> GetByTruckIdAsync(int truckId)
        {
            return await _dbSet
                .Include(i => i.Truck)
                .Include(i => i.Employee)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.TruckId == truckId && !i.IsCompleted);
        }

        public async Task<IEnumerable<Installment>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _dbSet
                .Where(i => i.EmployeeId == employeeId)
                .Include(i => i.Truck)
                .Include(i => i.Payments)
                .ToListAsync();
        }
    }
}
