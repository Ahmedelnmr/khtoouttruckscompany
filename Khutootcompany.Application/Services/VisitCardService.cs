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
    public class VisitCardService : IVisitCardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VisitCardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<VisitCardDto>> GetAllVisitCardsAsync()
        {
            var cards = await _unitOfWork.VisitCards.GetAllAsync();
            return cards.Select(MapToDto);
        }

        public async Task<VisitCardDto?> GetVisitCardByIdAsync(int id)
        {
            var card = await _unitOfWork.VisitCards.GetByIdAsync(id);
            return card != null ? MapToDto(card) : null;
        }

        public async Task<IEnumerable<VisitCardDto>> GetExpiredVisitCardsAsync()
        {
            var cards = await _unitOfWork.VisitCards.GetExpiredVisitCardsAsync();
            return cards.Select(MapToDto);
        }

        public async Task<IEnumerable<VisitCardDto>> GetExpiringSoonVisitCardsAsync(int days = 30)
        {
            var cards = await _unitOfWork.VisitCards.GetExpiringSoonVisitCardsAsync(days);
            return cards.Select(MapToDto);
        }

        public async Task<IEnumerable<VisitCardDto>> GetVisitCardsByEmployeeAsync(int employeeId)
        {
            var cards = await _unitOfWork.VisitCards.GetVisitCardsByEmployeeAsync(employeeId);
            return cards.Select(MapToDto);
        }

        public async Task<VisitCardDto> CreateVisitCardAsync(CreateVisitCardDto dto, string username)
        {
            var card = new VisitCard
            {
                EmployeeId = dto.EmployeeId,
                TruckId = dto.TruckId,
                IssueDate = dto.IssueDate,
                ExpiryDate = dto.ExpiryDate,
                Price = 15m, // Always 15 KD
                IsPaid = dto.IsPaid,
                IntermediaryName = dto.IntermediaryName,
                IntermediaryPhone = dto.IntermediaryPhone,
                SondNumber = dto.SondNumber,
                Notes = dto.Notes,
                CreatedBy = username,
                CreatedDate = DateTime.Now
            };

            await _unitOfWork.VisitCards.AddAsync(card);

            // Create cash transaction if paid
            if (dto.IsPaid)
            {
                var transaction = new CashTransaction
                {
                    TransactionDate = DateTime.Now,
                    Amount = -15m, // Negative = expense
                    Type = TransactionType.كارت_زيارة,
                    EmployeeId = dto.EmployeeId,
                    TruckId = dto.TruckId,
                    Description = $"كارت زيارة - {dto.EmployeeId}",
                    SondNumber = dto.SondNumber,
                    CreatedBy = username,
                    CreatedDate = DateTime.Now
                };
                await _unitOfWork.CashTransactions.AddAsync(transaction);
            }

            await _unitOfWork.SaveChangesAsync();

            return MapToDto(card);
        }

        public async Task<VisitCardDto> UpdateVisitCardAsync(VisitCardDto dto, string username)
        {
            var card = await _unitOfWork.VisitCards.GetByIdAsync(dto.VisitCardId);
            if (card == null)
                throw new KeyNotFoundException($"VisitCard with ID {dto.VisitCardId} not found");

            card.IssueDate = dto.IssueDate;
            card.ExpiryDate = dto.ExpiryDate;
            card.IsPaid = dto.IsPaid;
            card.IntermediaryName = dto.IntermediaryName;
            card.IntermediaryPhone = dto.IntermediaryPhone;
            card.SondNumber = dto.SondNumber;
            card.Notes = dto.Notes;
            card.UpdatedBy = username;
            card.UpdatedDate = DateTime.Now;

            _unitOfWork.VisitCards.Update(card);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(card);
        }

        public async Task DeleteVisitCardAsync(int id, string username)
        {
            var card = await _unitOfWork.VisitCards.GetByIdAsync(id);
            if (card == null)
                throw new KeyNotFoundException($"VisitCard with ID {id} not found");

            card.IsDeleted = true;
            card.DeletedBy = username;
            card.DeletedDate = DateTime.Now;

            _unitOfWork.VisitCards.Update(card);
            await _unitOfWork.SaveChangesAsync();
        }

        private VisitCardDto MapToDto(VisitCard card)
        {
            return new VisitCardDto
            {
                VisitCardId = card.VisitCardId,
                EmployeeId = card.EmployeeId,
                EmployeeName = card.Employee?.FullName ?? "",
                TruckId = card.TruckId,
                TruckPlate = card.Truck?.PlateNumber,
                IssueDate = card.IssueDate,
                ExpiryDate = card.ExpiryDate,
                Price = card.Price,
                IsPaid = card.IsPaid,
                IntermediaryName = card.IntermediaryName,
                IntermediaryPhone = card.IntermediaryPhone,
                SondNumber = card.SondNumber,
                Notes = card.Notes,
                IsExpired = card.IsExpired(),
                IsExpiringSoon = card.IsExpiringSoon(),
                IsWarning = card.IsWarning(),
                CreatedBy = card.CreatedBy,
                CreatedDate = card.CreatedDate
            };
        }
    }
}
