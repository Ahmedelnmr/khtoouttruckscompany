using Khutootcompany.Application.DTOs;
using Khutootcompany.Application.Interfaces;
using Khutootcompany.Domain.Entities;
using Khutootcompany.Domain.Enums;
using Khutootcompany.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.Services
{
    public class CashTransactionService : ICashTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CashTransactionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CashTransactionDto>> GetAllTransactionsAsync()
        {
            var transactions = await _unitOfWork.CashTransactions.GetAllAsync();
            return transactions.Select(MapToDto);
        }

        public async Task<CashTransactionDto?> GetTransactionByIdAsync(int id)
        {
            var transaction = await _unitOfWork.CashTransactions.GetByIdAsync(id);
            return transaction != null ? MapToDto(transaction) : null;
        }

        public async Task<IEnumerable<CashTransactionDto>> GetTransactionsByDateRangeAsync(DateTime start, DateTime end)
        {
            var transactions = await _unitOfWork.CashTransactions.GetTransactionsByDateRangeAsync(start, end);
            return transactions.Select(MapToDto);
        }

        public async Task<IEnumerable<CashTransactionDto>> GetTransactionsByTypeAsync(TransactionType type)
        {
            var transactions = await _unitOfWork.CashTransactions.GetTransactionsByTypeAsync(type);
            return transactions.Select(MapToDto);
        }

        public async Task<IEnumerable<CashTransactionDto>> GetTransactionsByEmployeeAsync(int employeeId)
        {
            var transactions = await _unitOfWork.CashTransactions.GetTransactionsByEmployeeAsync(employeeId);
            return transactions.Select(MapToDto);
        }

        public async Task<IEnumerable<CashTransactionDto>> GetTransactionsByTruckAsync(int truckId)
        {
            var transactions = await _unitOfWork.CashTransactions.GetTransactionsByTruckAsync(truckId);
            return transactions.Select(MapToDto);
        }

        public async Task<IEnumerable<CashTransactionDto>> GetLatestTransactionsAsync(int count = 10)
        {
            var transactions = await _unitOfWork.CashTransactions.GetLatestTransactionsAsync(count);
            return transactions.Select(MapToDto);
        }

        public async Task<IEnumerable<CashTransactionDto>> GetMonthlyTransactionsAsync(int year, int month)
        {
            var transactions = await _unitOfWork.CashTransactions.GetMonthlyTransactionsAsync(year, month);
            return transactions.Select(MapToDto);
        }

        public async Task<decimal> GetCurrentBalanceAsync()
        {
            return await _unitOfWork.CashTransactions.GetCurrentBalanceAsync();
        }

        public async Task<CashTransactionDto> CreateTransactionAsync(CreateCashTransactionDto dto, string username)
        {
            var transaction = new CashTransaction
            {
                TransactionDate = dto.TransactionDate,
                Amount = dto.Amount,
                Type = dto.Type,
                EmployeeId = dto.EmployeeId,
                TruckId = dto.TruckId,
                Description = dto.Description,
                SondNumber = dto.SondNumber,
                Notes = dto.Notes,
                CreatedBy = username,
                CreatedDate = DateTime.Now
            };

            await _unitOfWork.CashTransactions.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(transaction);
        }

        public async Task<CashTransactionDto> UpdateTransactionAsync(CreateCashTransactionDto dto, string username)
        {
            var transaction = await _unitOfWork.CashTransactions.GetByIdAsync(dto.Id);
            if (transaction == null)
                throw new KeyNotFoundException($"Transaction with ID {dto.Id} not found");

            transaction.TransactionDate = dto.TransactionDate;
            transaction.Amount = dto.Amount;
            transaction.Type = dto.Type;
            transaction.EmployeeId = dto.EmployeeId;
            transaction.TruckId = dto.TruckId;
            transaction.Description = dto.Description;
            transaction.SondNumber = dto.SondNumber;
            transaction.Notes = dto.Notes;
            transaction.UpdatedBy = username;
            transaction.UpdatedDate = DateTime.Now;

            _unitOfWork.CashTransactions.Update(transaction);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(transaction);
        }

        public async Task DeleteTransactionAsync(int id, string username)
        {
            var transaction = await _unitOfWork.CashTransactions.GetByIdAsync(id);
            if (transaction == null)
                throw new KeyNotFoundException($"Transaction with ID {id} not found");

            transaction.IsDeleted = true;
            transaction.DeletedBy = username;
            transaction.DeletedDate = DateTime.Now;

            _unitOfWork.CashTransactions.Update(transaction);
            await _unitOfWork.SaveChangesAsync();
        }

        private CashTransactionDto MapToDto(CashTransaction transaction)
        {
            return new CashTransactionDto
            {
                TransactionId = transaction.TransactionId,
                TransactionDate = transaction.TransactionDate,
                Amount = transaction.Amount,
                Type = transaction.Type,
                TypeDisplay = transaction.Type.ToString().Replace("_", " "),
                EmployeeId = transaction.EmployeeId,
                EmployeeName = transaction.Employee?.FullName,
                TruckId = transaction.TruckId,
                TruckPlate = transaction.Truck?.PlateNumber,
                Description = transaction.Description,
                SondNumber = transaction.SondNumber,
                Notes = transaction.Notes,
                IsIncome = transaction.IsIncome,
                IsExpense = transaction.IsExpense,
                CreatedBy = transaction.CreatedBy,
                CreatedDate = transaction.CreatedDate
            };
        }
    }
}
