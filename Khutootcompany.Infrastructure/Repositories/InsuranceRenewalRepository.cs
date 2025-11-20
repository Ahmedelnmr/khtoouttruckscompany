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
    public class InsuranceRenewalRepository : GenericRepository<InsuranceRenewal>, IInsuranceRenewalRepository
    {
        public InsuranceRenewalRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<InsuranceRenewal>> GetRenewalsByTruckAsync(int truckId)
        {
            return await _dbSet
                .Where(i => i.TruckId == truckId)
                .OrderByDescending(i => i.RenewalDate)
                .ToListAsync();
        }

        public async Task<InsuranceRenewal?> GetLatestRenewalForTruckAsync(int truckId)
        {
            return await _dbSet
                .Where(i => i.TruckId == truckId)
                .OrderByDescending(i => i.RenewalDate)
                .FirstOrDefaultAsync();
        }
    }
}
