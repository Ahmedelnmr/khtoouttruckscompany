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
    public class CashTransactionRepository : GenericRepository<CashTransaction>, ICashTransactionRepository
    {
        public CashTransactionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<CashTransaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .Include(t => t.Employee)
                .Include(t => t.Truck)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<CashTransaction>> GetTransactionsByTypeAsync(TransactionType type)
        {
            return await _dbSet
                .Where(t => t.Type == type)
                .Include(t => t.Employee)
                .Include(t => t.Truck)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<CashTransaction>> GetTransactionsByEmployeeAsync(int employeeId)
        {
            return await _dbSet
                .Where(t => t.EmployeeId == employeeId)
                .Include(t => t.Truck)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<CashTransaction>> GetTransactionsByTruckAsync(int truckId)
        {
            return await _dbSet
                .Where(t => t.TruckId == truckId)
                .Include(t => t.Employee)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<decimal> GetCurrentBalanceAsync()
        {
            var total = await _dbSet.SumAsync(t => t.Amount);
            return total;
        }

        public async Task<IEnumerable<CashTransaction>> GetLatestTransactionsAsync(int count = 10)
        {
            return await _dbSet
                .Include(t => t.Employee)
                .Include(t => t.Truck)
                .OrderByDescending(t => t.TransactionDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<CashTransaction>> GetMonthlyTransactionsAsync(int year, int month)
        {
            return await _dbSet
                .Where(t => t.TransactionDate.Year == year && t.TransactionDate.Month == month)
                .Include(t => t.Employee)
                .Include(t => t.Truck)
                .OrderBy(t => t.TransactionDate)
                .ToListAsync();
        }
    }
}
