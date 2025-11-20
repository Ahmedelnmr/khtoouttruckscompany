using Khutootcompany.Domain.Entities;
using Khutootcompany.Domain.Enums;
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
    public class TruckRepository : GenericRepository<Truck>, ITruckRepository
    {
        public TruckRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Truck>> GetTrucksWithCurrentDriverAsync()
        {
            return await _dbSet
                .Include(t => t.Assignments.Where(a => a.IsCurrent))
                    .ThenInclude(a => a.Employee)
                .ToListAsync();
        }

        public async Task<IEnumerable<Truck>> GetTrucksWithExpiredInsuranceAsync()
        {
            var today = DateTime.Today;
            return await _dbSet
                .Where(t => t.InsuranceExpiryDate.HasValue && t.InsuranceExpiryDate.Value < today)
                .Include(t => t.Assignments.Where(a => a.IsCurrent))
                    .ThenInclude(a => a.Employee)
                .ToListAsync();
        }

        public async Task<IEnumerable<Truck>> GetTrucksWithExpiringSoonInsuranceAsync(int daysThreshold = 30)
        {
            var today = DateTime.Today;
            var futureDate = today.AddDays(daysThreshold);
            return await _dbSet
                .Where(t => t.InsuranceExpiryDate.HasValue &&
                           t.InsuranceExpiryDate.Value >= today &&
                           t.InsuranceExpiryDate.Value <= futureDate)
                .Include(t => t.Assignments.Where(a => a.IsCurrent))
                    .ThenInclude(a => a.Employee)
                .ToListAsync();
        }

        public async Task<IEnumerable<Truck>> GetTrucksByPAMStatusAsync(PAMStatus status)
        {
            return await _dbSet
                .Where(t => t.PAMStatus == status)
                .Include(t => t.Assignments.Where(a => a.IsCurrent))
                    .ThenInclude(a => a.Employee)
                .ToListAsync();
        }

        public async Task<IEnumerable<Truck>> GetTrucksByLicenseTypeAsync(LicenseType licenseType)
        {
            return await _dbSet
                .Where(t => t.LicenseType == licenseType)
                .Include(t => t.Assignments.Where(a => a.IsCurrent))
                    .ThenInclude(a => a.Employee)
                .ToListAsync();
        }

        public async Task<Truck?> GetTruckWithDetailsAsync(int truckId)
        {
            return await _dbSet
                .Include(t => t.Assignments.Where(a => a.IsCurrent))
                    .ThenInclude(a => a.Employee)
                .Include(t => t.Installments)
                    .ThenInclude(i => i.Payments)
                .Include(t => t.Wakalat)
                .Include(t => t.InsuranceRenewals)
                .FirstOrDefaultAsync(t => t.TruckId == truckId);
        }

        public async Task<Truck?> GetByPlateNumberAsync(string plateNumber)
        {
            return await _dbSet
                .Include(t => t.Assignments.Where(a => a.IsCurrent))
                    .ThenInclude(a => a.Employee)
                .FirstOrDefaultAsync(t => t.PlateNumber == plateNumber);
        }
    }
}
