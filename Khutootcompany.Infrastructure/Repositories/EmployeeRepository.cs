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
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Employee>> GetDriversAsync()
        {
            return await _dbSet
                .Where(e => e.IsDriver)
                .Include(e => e.TruckAssignments.Where(a => a.IsCurrent))
                    .ThenInclude(a => a.Truck)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetAdminStaffAsync()
        {
            return await _dbSet
                .Where(e => !e.IsDriver)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesWithExpiredResidencyAsync()
        {
            var today = DateTime.Today;
            return await _dbSet
                .Where(e => e.ResidencyExpiryDate.HasValue && e.ResidencyExpiryDate.Value < today)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesWithExpiringSoonResidencyAsync(int daysThreshold = 30)
        {
            var today = DateTime.Today;
            var futureDate = today.AddDays(daysThreshold);
            return await _dbSet
                .Where(e => e.ResidencyExpiryDate.HasValue &&
                           e.ResidencyExpiryDate.Value >= today &&
                           e.ResidencyExpiryDate.Value <= futureDate)
                .ToListAsync();
        }

        public async Task<Employee?> GetEmployeeWithDetailsAsync(int employeeId)
        {
            return await _dbSet
                .Include(e => e.TruckAssignments.Where(a => a.IsCurrent))
                    .ThenInclude(a => a.Truck)
                .Include(e => e.Installments)
                .Include(e => e.Wakalat)
                .Include(e => e.VisitCards)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }

        public async Task<Employee?> GetByCivilIdAsync(string civilId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(e => e.CivilId == civilId);
        }

        public async Task<Employee?> GetCurrentDriverForTruckAsync(int truckId)
        {
            return await _dbSet
                .Include(e => e.TruckAssignments)
                .FirstOrDefaultAsync(e => e.TruckAssignments.Any(a => a.TruckId == truckId && a.IsCurrent));
        }
    }
}
