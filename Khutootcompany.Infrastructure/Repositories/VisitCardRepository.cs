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
    public class VisitCardRepository : GenericRepository<VisitCard>, IVisitCardRepository
    {
        public VisitCardRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<VisitCard>> GetExpiredVisitCardsAsync()
        {
            var today = DateTime.Today;
            return await _dbSet
                .Where(v => v.ExpiryDate < today)
                .Include(v => v.Employee)
                .Include(v => v.Truck)
                .ToListAsync();
        }

        public async Task<IEnumerable<VisitCard>> GetExpiringSoonVisitCardsAsync(int daysThreshold = 30)
        {
            var today = DateTime.Today;
            var futureDate = today.AddDays(daysThreshold);
            return await _dbSet
                .Where(v => v.ExpiryDate >= today && v.ExpiryDate <= futureDate)
                .Include(v => v.Employee)
                .Include(v => v.Truck)
                .ToListAsync();
        }

        public async Task<IEnumerable<VisitCard>> GetVisitCardsByEmployeeAsync(int employeeId)
        {
            return await _dbSet
                .Where(v => v.EmployeeId == employeeId)
                .Include(v => v.Truck)
                .OrderByDescending(v => v.IssueDate)
                .ToListAsync();
        }
    }
}
