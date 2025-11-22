using Khutootcompany.Application.DTOs;
using Khutootcompany.Application.Interfaces;
using Khutootcompany.Domain.Entities;
using Khutootcompany.Domain.Enums;
using Khutootcompany.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Khutootcompany.Application.Services
{
    public class WakalaService : IWakalaService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WakalaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<WakalaDto>> GetAllWakalatAsync()
        {
            var wakalat = await _unitOfWork.Wakalat.GetAllAsync();
            return wakalat.Select(MapToDto);
        }

        public async Task<WakalaDto?> GetWakalaByIdAsync(int id)
        {
            var wakala = await _unitOfWork.Wakalat.GetByIdAsync(id);
            return wakala != null ? MapToDto(wakala) : null;
        }

        public async Task<IEnumerable<WakalaDto>> GetExpiredWakalatAsync()
        {
            var wakalat = await _unitOfWork.Wakalat.GetExpiredWakalatAsync();
            return wakalat.Select(MapToDto);
        }

        public async Task<IEnumerable<WakalaDto>> GetExpiringSoonWakalatAsync(int days = 30)
        {
            var wakalat = await _unitOfWork.Wakalat.GetExpiringSoonWakalatAsync(days);
            return wakalat.Select(MapToDto);
        }

        public async Task<IEnumerable<WakalaDto>> GetGeneralWakalatAsync()
        {
            var wakalat = await _unitOfWork.Wakalat.GetGeneralWakalatAsync();
            return wakalat.Select(MapToDto);
        }

        public async Task<IEnumerable<WakalaDto>> GetWakalatByEmployeeAsync(int employeeId)
        {
            var wakalat = await _unitOfWork.Wakalat.GetWakalatByEmployeeAsync(employeeId);
            return wakalat.Select(MapToDto);
        }

        public async Task<IEnumerable<WakalaDto>> GetWakalatByTruckAsync(int truckId)
        {
            var wakalat = await _unitOfWork.Wakalat.GetWakalatByTruckAsync(truckId);
            return wakalat.Select(MapToDto);
        }

        // ⭐ CREATE - محسّن مع validation
        public async Task<WakalaDto> CreateWakalaAsync(WakalaDto dto, string username)
        {
            // Validation
            if (!dto.IsGeneral && !dto.TruckId.HasValue)
                throw new InvalidOperationException("يجب تحديد الشاحنة للوكالة غير العامة");

            if (dto.ExpiryDate <= dto.IssueDate)
                throw new InvalidOperationException("تاريخ الانتهاء يجب أن يكون بعد تاريخ الإصدار");

            var wakala = new Wakala
            {
                EmployeeId = dto.EmployeeId,
                TruckId = dto.TruckId,
                IssueDate = dto.IssueDate,
                ExpiryDate = dto.ExpiryDate,
                IsGeneral = dto.IsGeneral,
                IsPaid = dto.IsPaid,
                Price = dto.Price,
                SondNumber = dto.SondNumber,
                Notes = dto.Notes,
                CreatedBy = username,
                CreatedDate = DateTime.Now
            };

            await _unitOfWork.Wakalat.AddAsync(wakala);

            // ⭐ Create cash transaction if paid
            if (dto.IsPaid && dto.Price > 0)
            {
                var transaction = new CashTransaction
                {
                    TransactionDate = DateTime.Now,
                    Amount = -dto.Price, // Negative = expense
                    Type = TransactionType.وكالة,
                    EmployeeId = dto.EmployeeId,
                    TruckId = dto.TruckId,
                    Description = dto.IsGeneral
                        ? $"وكالة عامة - {dto.EmployeeName}"
                        : $"وكالة شاحنة {dto.TruckPlate}",
                    SondNumber = dto.SondNumber,
                    CreatedBy = username,
                    CreatedDate = DateTime.Now
                };
                await _unitOfWork.CashTransactions.AddAsync(transaction);
            }

            await _unitOfWork.SaveChangesAsync();

            return MapToDto(wakala);
        }

        // ⭐ UPDATE - محسّن
        public async Task<WakalaDto> UpdateWakalaAsync(WakalaDto dto, string username)
        {
            var wakala = await _unitOfWork.Wakalat.GetByIdAsync(dto.WakalaId);
            if (wakala == null)
                throw new KeyNotFoundException($"الوكالة رقم {dto.WakalaId} غير موجودة");

            // Validation
            if (!dto.IsGeneral && !dto.TruckId.HasValue)
                throw new InvalidOperationException("يجب تحديد الشاحنة للوكالة غير العامة");

            if (dto.ExpiryDate <= dto.IssueDate)
                throw new InvalidOperationException("تاريخ الانتهاء يجب أن يكون بعد تاريخ الإصدار");

            // Track if payment status changed
            bool paymentChanged = wakala.IsPaid != dto.IsPaid;
            bool wasUnpaidNowPaid = !wakala.IsPaid && dto.IsPaid;

            wakala.IssueDate = dto.IssueDate;
            wakala.ExpiryDate = dto.ExpiryDate;
            wakala.IsPaid = dto.IsPaid;
            wakala.Price = dto.Price;
            wakala.SondNumber = dto.SondNumber;
            wakala.Notes = dto.Notes;
            wakala.UpdatedBy = username;
            wakala.UpdatedDate = DateTime.Now;

            _unitOfWork.Wakalat.Update(wakala);

            // ⭐ إذا تم تغيير حالة الدفع من غير مدفوع لمدفوع
            if (wasUnpaidNowPaid && dto.Price > 0)
            {
                var transaction = new CashTransaction
                {
                    TransactionDate = DateTime.Now,
                    Amount = -dto.Price,
                    Type = TransactionType.وكالة,
                    EmployeeId = dto.EmployeeId,
                    TruckId = dto.TruckId,
                    Description = dto.IsGeneral
                        ? $"دفع وكالة عامة - {dto.EmployeeName}"
                        : $"دفع وكالة شاحنة {dto.TruckPlate}",
                    SondNumber = dto.SondNumber,
                    CreatedBy = username,
                    CreatedDate = DateTime.Now
                };
                await _unitOfWork.CashTransactions.AddAsync(transaction);
            }

            await _unitOfWork.SaveChangesAsync();

            return MapToDto(wakala);
        }

        // ⭐ DELETE
        public async Task DeleteWakalaAsync(int id, string username)
        {
            var wakala = await _unitOfWork.Wakalat.GetByIdAsync(id);
            if (wakala == null)
                throw new KeyNotFoundException($"الوكالة رقم {id} غير موجودة");

            wakala.IsDeleted = true;
            wakala.DeletedBy = username;
            wakala.DeletedDate = DateTime.Now;

            _unitOfWork.Wakalat.Update(wakala);
            await _unitOfWork.SaveChangesAsync();
        }

        // ⭐ MAPPING
        private WakalaDto MapToDto(Wakala wakala)
        {
            return new WakalaDto
            {
                WakalaId = wakala.WakalaId,
                EmployeeId = wakala.EmployeeId,
                EmployeeName = wakala.Employee?.FullName ?? "",
                TruckId = wakala.TruckId,
                TruckPlate = wakala.Truck?.PlateNumber,
                IssueDate = wakala.IssueDate,
                ExpiryDate = wakala.ExpiryDate,
                IsGeneral = wakala.IsGeneral,
                IsPaid = wakala.IsPaid,
                Price = wakala.Price,
                SondNumber = wakala.SondNumber,
                Notes = wakala.Notes,
                IsExpired = wakala.IsExpired(),
                IsExpiringSoon = wakala.IsExpiringSoon(),
                IsWarning = wakala.IsWarning(),
                CreatedBy = wakala.CreatedBy,
                CreatedDate = wakala.CreatedDate,
                UpdatedBy = wakala.UpdatedBy,
                UpdatedDate = wakala.UpdatedDate
            };
        }
    }
}