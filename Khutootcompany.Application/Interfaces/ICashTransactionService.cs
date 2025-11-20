using Khutootcompany.Application.DTOs;
using Khutootcompany.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.Interfaces
{
    public interface ICashTransactionService
    {
        Task<IEnumerable<CashTransactionDto>> GetAllTransactionsAsync();
        Task<CashTransactionDto?> GetTransactionByIdAsync(int id);
        Task<IEnumerable<CashTransactionDto>> GetTransactionsByDateRangeAsync(DateTime start, DateTime end);
        Task<IEnumerable<CashTransactionDto>> GetTransactionsByTypeAsync(TransactionType type);
        Task<IEnumerable<CashTransactionDto>> GetTransactionsByEmployeeAsync(int employeeId);
        Task<IEnumerable<CashTransactionDto>> GetTransactionsByTruckAsync(int truckId);
        Task<IEnumerable<CashTransactionDto>> GetLatestTransactionsAsync(int count = 10);
        Task<IEnumerable<CashTransactionDto>> GetMonthlyTransactionsAsync(int year, int month);
        Task<decimal> GetCurrentBalanceAsync();
        Task<CashTransactionDto> CreateTransactionAsync(CreateCashTransactionDto dto, string username);
        Task<CashTransactionDto> UpdateTransactionAsync(CreateCashTransactionDto dto, string username);
        Task DeleteTransactionAsync(int id, string username);
    }
}
