using Khutootcompany.Application.DTOs;
using Khutootcompany.Application.Interfaces;
using Khutootcompany.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.Services
{

    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardDto> GetDashboardDataAsync()
        {
            // Total trucks
            var totalTrucks = await _unitOfWork.Trucks.CountAsync();

            // Expired residencies
            var expiredResidencies = (await _unitOfWork.Employees.GetEmployeesWithExpiredResidencyAsync()).Count();
            expiredResidencies += (await _unitOfWork.Employees.GetEmployeesWithExpiringSoonResidencyAsync(30)).Count();

            // Expired insurance
            var expiredInsurance = (await _unitOfWork.Trucks.GetTrucksWithExpiredInsuranceAsync()).Count();
            expiredInsurance += (await _unitOfWork.Trucks.GetTrucksWithExpiringSoonInsuranceAsync(30)).Count();

            // Overdue installments
            var overdueInstallments = (await _unitOfWork.Installments.GetOverdueInstallmentsAsync()).Count();

            // Cash balance
            var cashBalance = await _unitOfWork.CashTransactions.GetCurrentBalanceAsync();

            // Expired wakalat
            var expiredWakalat = (await _unitOfWork.Wakalat.GetExpiredWakalatAsync()).Count();
            expiredWakalat += (await _unitOfWork.Wakalat.GetExpiringSoonWakalatAsync(30)).Count();

            // Active drivers
            var activeDrivers = (await _unitOfWork.Employees.GetDriversAsync()).Count();

            // Admin staff
            var adminStaff = (await _unitOfWork.Employees.GetAdminStaffAsync()).Count();

            // Latest transactions
            var latestTransactions = await _unitOfWork.CashTransactions.GetLatestTransactionsAsync(10);
            var transactionDtos = latestTransactions.Select(t => new CashTransactionDto
            {
                TransactionId = t.TransactionId,
                TransactionDate = t.TransactionDate,
                Amount = t.Amount,
                Type = t.Type,
                TypeDisplay = t.Type.ToString().Replace("_", " "),
                EmployeeName = t.Employee?.FullName,
                TruckPlate = t.Truck?.PlateNumber,
                Description = t.Description,
                SondNumber = t.SondNumber,
                IsIncome = t.IsIncome,
                IsExpense = t.IsExpense
            }).ToList();

            return new DashboardDto
            {
                TotalTrucks = totalTrucks,
                ExpiredResidencies = expiredResidencies,
                ExpiredInsurance = expiredInsurance,
                OverdueInstallments = overdueInstallments,
                CurrentCashBalance = cashBalance,
                ExpiredWakalat = expiredWakalat,
                ActiveDrivers = activeDrivers,
                AdminStaff = adminStaff,
                LatestTransactions = transactionDtos
            };
        }
    }
}
