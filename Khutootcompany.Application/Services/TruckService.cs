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
    public class TruckService : ITruckService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TruckService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TruckDto>> GetAllTrucksAsync()
        {
            var trucks = await _unitOfWork.Trucks.GetTrucksWithCurrentDriverAsync();
            return trucks.Select(MapToDto);
        }

        public async Task<TruckDto?> GetTruckByIdAsync(int id)
        {
            var truck = await _unitOfWork.Trucks.GetTruckWithDetailsAsync(id);
            return truck != null ? MapToDto(truck) : null;
        }

        public async Task<TruckDto?> GetTruckByPlateAsync(string plate)
        {
            var truck = await _unitOfWork.Trucks.GetByPlateNumberAsync(plate);
            return truck != null ? MapToDto(truck) : null;
        }

        public async Task<IEnumerable<TruckDto>> GetTrucksWithExpiredInsuranceAsync()
        {
            var trucks = await _unitOfWork.Trucks.GetTrucksWithExpiredInsuranceAsync();
            return trucks.Select(MapToDto);
        }

        public async Task<IEnumerable<TruckDto>> GetTrucksWithExpiringSoonInsuranceAsync(int days = 30)
        {
            var trucks = await _unitOfWork.Trucks.GetTrucksWithExpiringSoonInsuranceAsync(days);
            return trucks.Select(MapToDto);
        }

        public async Task<IEnumerable<TruckDto>> GetTrucksByPAMStatusAsync(PAMStatus status)
        {
            var trucks = await _unitOfWork.Trucks.GetTrucksByPAMStatusAsync(status);
            return trucks.Select(MapToDto);
        }

        public async Task<IEnumerable<TruckDto>> GetTrucksByLicenseTypeAsync(LicenseType licenseType)
        {
            var trucks = await _unitOfWork.Trucks.GetTrucksByLicenseTypeAsync(licenseType);
            return trucks.Select(MapToDto);
        }

        public async Task<TruckDto> CreateTruckAsync(TruckDto dto, string username)
        {
            var truck = new Truck
            {
                PlateNumber = dto.PlateNumber,
                AdminNumber = dto.AdminNumber,
                Model = dto.Model,
                Year = dto.Year,
                Color = dto.Color,
                LicenseType = dto.LicenseType,
                InsuranceExpiryDate = dto.InsuranceExpiryDate,
                InsuranceDay = dto.InsuranceDay,
                InsuranceMonth = dto.InsuranceMonth,
                InsuranceYear = dto.InsuranceYear,
                IsRegisteredInPAM = dto.IsRegisteredInPAM,
                PAMStatus = dto.PAMStatus,
                PAMRegistrationDate = dto.PAMRegistrationDate,
                PreviousPlateNumber = dto.PreviousPlateNumber,
                Notes = dto.Notes,
                CreatedBy = username,
                CreatedDate = DateTime.Now
            };

            await _unitOfWork.Trucks.AddAsync(truck);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(truck);
        }

        public async Task<TruckDto> UpdateTruckAsync(TruckDto dto, string username)
        {
            var truck = await _unitOfWork.Trucks.GetByIdAsync(dto.TruckId);
            if (truck == null)
                throw new KeyNotFoundException($"Truck with ID {dto.TruckId} not found");

            truck.PlateNumber = dto.PlateNumber;
            truck.AdminNumber = dto.AdminNumber;
            truck.Model = dto.Model;
            truck.Year = dto.Year;
            truck.Color = dto.Color;
            truck.LicenseType = dto.LicenseType;
            truck.InsuranceExpiryDate = dto.InsuranceExpiryDate;
            truck.InsuranceDay = dto.InsuranceDay;
            truck.InsuranceMonth = dto.InsuranceMonth;
            truck.InsuranceYear = dto.InsuranceYear;
            truck.IsRegisteredInPAM = dto.IsRegisteredInPAM;
            truck.PAMStatus = dto.PAMStatus;
            truck.PAMRegistrationDate = dto.PAMRegistrationDate;
            truck.PreviousPlateNumber = dto.PreviousPlateNumber;
            truck.Notes = dto.Notes;
            truck.UpdatedBy = username;
            truck.UpdatedDate = DateTime.Now;

            _unitOfWork.Trucks.Update(truck);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(truck);
        }

        public async Task DeleteTruckAsync(int id, string username)
        {
            var truck = await _unitOfWork.Trucks.GetByIdAsync(id);
            if (truck == null)
                throw new KeyNotFoundException($"Truck with ID {id} not found");

            truck.IsDeleted = true;
            truck.DeletedBy = username;
            truck.DeletedDate = DateTime.Now;

            _unitOfWork.Trucks.Update(truck);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AssignDriverAsync(int truckId, int employeeId, string username)
        {
            var truck = await _unitOfWork.Trucks.GetByIdAsync(truckId);
            if (truck == null)
                throw new KeyNotFoundException($"Truck with ID {truckId} not found");

            var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);
            if (employee == null || !employee.IsDriver)
                throw new InvalidOperationException("Employee must be a driver");

            // End current assignment if exists
            var currentAssignments = await _unitOfWork.TruckAssignments.FindAsync(
                a => a.TruckId == truckId && a.IsCurrent);

            foreach (var assignment in currentAssignments)
            {
                assignment.IsCurrent = false;
                assignment.EndDate = DateTime.Now;
                _unitOfWork.TruckAssignments.Update(assignment);
            }

            // Create new assignment
            var newAssignment = new TruckAssignment
            {
                TruckId = truckId,
                EmployeeId = employeeId,
                AssignmentDate = DateTime.Now,
                IsCurrent = true,
                CreatedBy = username,
                CreatedDate = DateTime.Now
            };

            await _unitOfWork.TruckAssignments.AddAsync(newAssignment);
            await _unitOfWork.SaveChangesAsync();
        }

        private TruckDto MapToDto(Truck truck)
        {
            var currentAssignment = truck.Assignments
                .FirstOrDefault(a => a.IsCurrent);

            return new TruckDto
            {
                TruckId = truck.TruckId,
                PlateNumber = truck.PlateNumber,
                AdminNumber = truck.AdminNumber,
                Model = truck.Model,
                Year = truck.Year,
                Color = truck.Color,
                LicenseType = truck.LicenseType,
                LicenseTypeDisplay = truck.LicenseType.ToString().Replace("_", " "),
                InsuranceExpiryDate = truck.InsuranceExpiryDate,
                InsuranceDay = truck.InsuranceDay,
                InsuranceMonth = truck.InsuranceMonth,
                InsuranceYear = truck.InsuranceYear,
                IsRegisteredInPAM = truck.IsRegisteredInPAM,
                PAMStatus = truck.PAMStatus,
                PAMStatusDisplay = truck.PAMStatus?.ToString().Replace("_", " "),
                PAMRegistrationDate = truck.PAMRegistrationDate,
                PreviousPlateNumber = truck.PreviousPlateNumber,
                Notes = truck.Notes,
                CurrentDriverName = currentAssignment?.Employee?.FullName,
                CurrentDriverCivilId = currentAssignment?.Employee?.CivilId,
                CurrentDriverNationality = currentAssignment?.Employee?.Nationality?.ToString().Replace("_", " "),
                IsInsuranceExpired = truck.IsInsuranceExpired(),
                IsInsuranceExpiringSoon = truck.IsInsuranceExpiringSoon(),
                IsInsuranceWarning = truck.IsInsuranceWarning(),
                CreatedBy = truck.CreatedBy,
                CreatedDate = truck.CreatedDate,
                UpdatedBy = truck.UpdatedBy,
                UpdatedDate = truck.UpdatedDate
            };
        }
    }
}
