using Khutootcompany.Domain.Entities;
using Khutootcompany.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Domain.Interfaces
{
    public interface ICashTransactionRepository : IRepository<CashTransaction>
    {
        Task<IEnumerable<CashTransaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<CashTransaction>> GetTransactionsByTypeAsync(TransactionType type);
        Task<IEnumerable<CashTransaction>> GetTransactionsByEmployeeAsync(int employeeId);
        Task<IEnumerable<CashTransaction>> GetTransactionsByTruckAsync(int truckId);
        Task<decimal> GetCurrentBalanceAsync();
        Task<IEnumerable<CashTransaction>> GetLatestTransactionsAsync(int count = 10);
        Task<IEnumerable<CashTransaction>> GetMonthlyTransactionsAsync(int year, int month);
    }
}
