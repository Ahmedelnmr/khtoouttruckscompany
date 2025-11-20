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

    public class InsuranceRenewalService : IInsuranceRenewalService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InsuranceRenewalService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task RenewInsuranceAsync(int truckId, DateTime expiryDate, decimal cost, string? sondNumber, string username)
        {
            var truck = await _unitOfWork.Trucks.GetByIdAsync(truckId);
            if (truck == null)
                throw new KeyNotFoundException($"Truck with ID {truckId} not found");

            // Create renewal record
            var renewal = new InsuranceRenewal
            {
                TruckId = truckId,
                RenewalDate = DateTime.Now,
                ExpiryDate = expiryDate,
                Cost = cost,
                SondNumber = sondNumber,
                CreatedBy = username,
                CreatedDate = DateTime.Now
            };

            await _unitOfWork.InsuranceRenewals.AddAsync(renewal);

            // Update truck insurance date
            truck.InsuranceExpiryDate = expiryDate;
            truck.InsuranceDay = expiryDate.Day;
            truck.InsuranceMonth = expiryDate.Month;
            truck.InsuranceYear = expiryDate.Year;
            truck.UpdatedBy = username;
            truck.UpdatedDate = DateTime.Now;

            _unitOfWork.Trucks.Update(truck);

            // Create cash transaction
            var transaction = new CashTransaction
            {
                TransactionDate = DateTime.Now,
                Amount = -cost, // Negative = expense
                Type = TransactionType.تأمين,
                TruckId = truckId,
                Description = $"تجديد تأمين شاحنة {truck.PlateNumber}",
                SondNumber = sondNumber,
                CreatedBy = username,
                CreatedDate = DateTime.Now
            };

            await _unitOfWork.CashTransactions.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<InsuranceRenewal>> GetRenewalsByTruckAsync(int truckId)
        {
            var renewals = await _unitOfWork.InsuranceRenewals.GetRenewalsByTruckAsync(truckId);
            return renewals.Select(r => new InsuranceRenewal
            {
                InsuranceId = r.InsuranceId,
                RenewalDate = r.RenewalDate,
                ExpiryDate = r.ExpiryDate,
                Cost = r.Cost,
                SondNumber = r.SondNumber
            });
        }
    }
}
