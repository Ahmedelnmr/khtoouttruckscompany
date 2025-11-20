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
    public class InstallmentService : IInstallmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InstallmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<InstallmentDto>> GetAllInstallmentsAsync()
        {
            var installments = await _unitOfWork.Installments.GetAllAsync();
            return installments.Select(MapToDto);
        }

        public async Task<InstallmentDto?> GetInstallmentByIdAsync(int id)
        {
            var installment = await _unitOfWork.Installments.GetInstallmentWithPaymentsAsync(id);
            return installment != null ? MapToDto(installment) : null;
        }

        public async Task<IEnumerable<InstallmentDto>> GetActiveInstallmentsAsync()
        {
            var installments = await _unitOfWork.Installments.GetActiveInstallmentsAsync();
            return installments.Select(MapToDto);
        }

        public async Task<IEnumerable<InstallmentDto>> GetOverdueInstallmentsAsync()
        {
            var installments = await _unitOfWork.Installments.GetOverdueInstallmentsAsync();
            return installments.Select(MapToDto);
        }

        public async Task<InstallmentDto?> GetInstallmentByTruckIdAsync(int truckId)
        {
            var installment = await _unitOfWork.Installments.GetByTruckIdAsync(truckId);
            return installment != null ? MapToDto(installment) : null;
        }

        public async Task<IEnumerable<InstallmentDto>> GetInstallmentsByEmployeeIdAsync(int employeeId)
        {
            var installments = await _unitOfWork.Installments.GetByEmployeeIdAsync(employeeId);
            return installments.Select(MapToDto);
        }

        public async Task<InstallmentDto> CreateInstallmentAsync(InstallmentDto dto, string username)
        {
            var installment = new Installment
            {
                TruckId = dto.TruckId,
                EmployeeId = dto.EmployeeId,
                TotalPriceOffice = dto.TotalPriceOffice,
                TotalPriceSayer = dto.TotalPriceSayer,
                MonthlyQest = dto.MonthlyQest,
                PaidAmount = 0,
                RemainingAmount = dto.TotalPriceOffice,
                FinanceSource = dto.FinanceSource,
                SayerTransactionNumber = dto.SayerTransactionNumber,
                StartDate = dto.StartDate,
                TotalMonths = dto.TotalMonths,
                IsCompleted = false,
                Notes = dto.Notes,
                CreatedBy = username,
                CreatedDate = DateTime.Now
            };

            await _unitOfWork.Installments.AddAsync(installment);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(installment);
        }

        public async Task<InstallmentDto> UpdateInstallmentAsync(InstallmentDto dto, string username)
        {
            var installment = await _unitOfWork.Installments.GetByIdAsync(dto.InstallmentId);
            if (installment == null)
                throw new KeyNotFoundException($"Installment with ID {dto.InstallmentId} not found");

            installment.MonthlyQest = dto.MonthlyQest;
            installment.FinanceSource = dto.FinanceSource;
            installment.Notes = dto.Notes;
            installment.UpdatedBy = username;
            installment.UpdatedDate = DateTime.Now;

            _unitOfWork.Installments.Update(installment);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(installment);
        }

        public async Task RecordPaymentAsync(int installmentId, decimal amount, string paymentType, string? sondNumber, string username)
        {
            var installment = await _unitOfWork.Installments.GetByIdAsync(installmentId);
            if (installment == null)
                throw new KeyNotFoundException($"Installment with ID {installmentId} not found");

            // Create payment record
            var payment = new InstallmentPayment
            {
                InstallmentId = installmentId,
                PaymentDate = DateTime.Now,
                Amount = amount,
                PaymentType = paymentType,
                SondNumber = sondNumber,
                CreatedBy = username,
                CreatedDate = DateTime.Now
            };

            await _unitOfWork.InstallmentPayments.AddAsync(payment);

            // Update installment
            installment.PaidAmount += amount;
            installment.RemainingAmount = installment.TotalPriceOffice - installment.PaidAmount;

            if (installment.RemainingAmount <= 0)
            {
                installment.IsCompleted = true;
                installment.RemainingAmount = 0;
            }

            installment.UpdatedBy = username;
            installment.UpdatedDate = DateTime.Now;

            _unitOfWork.Installments.Update(installment);

            // Create cash transaction
            var transaction = new CashTransaction
            {
                TransactionDate = DateTime.Now,
                Amount = amount,
                Type = TransactionType.قسط,
                EmployeeId = installment.EmployeeId,
                TruckId = installment.TruckId,
                Description = $"قسط شهري - {paymentType}",
                SondNumber = sondNumber,
                CreatedBy = username,
                CreatedDate = DateTime.Now
            };

            await _unitOfWork.CashTransactions.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteInstallmentAsync(int id, string username)
        {
            var installment = await _unitOfWork.Installments.GetByIdAsync(id);
            if (installment == null)
                throw new KeyNotFoundException($"Installment with ID {id} not found");

            installment.IsDeleted = true;
            installment.DeletedBy = username;
            installment.DeletedDate = DateTime.Now;

            _unitOfWork.Installments.Update(installment);
            await _unitOfWork.SaveChangesAsync();
        }

        private InstallmentDto MapToDto(Installment installment)
        {
            return new InstallmentDto
            {
                InstallmentId = installment.InstallmentId,
                TruckId = installment.TruckId,
                TruckPlate = installment.Truck?.PlateNumber ?? "",
                EmployeeId = installment.EmployeeId,
                EmployeeName = installment.Employee?.FullName ?? "",
                TotalPriceOffice = installment.TotalPriceOffice,
                TotalPriceSayer = installment.TotalPriceSayer,
                ProfitMargin = installment.ProfitMargin,
                MonthlyQest = installment.MonthlyQest,
                PaidAmount = installment.PaidAmount,
                RemainingAmount = installment.RemainingAmount,
                ProgressPercentage = installment.TotalPriceOffice > 0
                    ? (installment.PaidAmount / installment.TotalPriceOffice) * 100
                    : 0,
                FinanceSource = installment.FinanceSource,
                SayerTransactionNumber = installment.SayerTransactionNumber,
                StartDate = installment.StartDate,
                TotalMonths = installment.TotalMonths,
                IsCompleted = installment.IsCompleted,
                IsOverdue = installment.IsOverdue(),
                Notes = installment.Notes,
                Payments = installment.Payments?.Select(p => new InstallmentPaymentDto
                {
                    PaymentId = p.PaymentId,
                    PaymentDate = p.PaymentDate,
                    Amount = p.Amount,
                    PaymentType = p.PaymentType,
                    SondNumber = p.SondNumber,
                    Notes = p.Notes
                }).ToList() ?? new List<InstallmentPaymentDto>(),
                CreatedBy = installment.CreatedBy,
                CreatedDate = installment.CreatedDate
            };
        }
    }
}
