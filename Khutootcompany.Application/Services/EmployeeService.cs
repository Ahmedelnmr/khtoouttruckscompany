using Khutootcompany.Application.DTOs;
using Khutootcompany.Application.Interfaces;
using Khutootcompany.Domain.Entities;
using Khutootcompany.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            var employees = await _unitOfWork.Employees.GetAllAsync();
            return employees.Select(MapToDto);
        }

        public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
        {
            var employee = await _unitOfWork.Employees.GetEmployeeWithDetailsAsync(id);
            return employee != null ? MapToDto(employee) : null;
        }

        public async Task<EmployeeDto?> GetEmployeeByCivilIdAsync(string civilId)
        {
            var employee = await _unitOfWork.Employees.GetByCivilIdAsync(civilId);
            return employee != null ? MapToDto(employee) : null;
        }

        public async Task<IEnumerable<EmployeeDto>> GetDriversAsync()
        {
            var drivers = await _unitOfWork.Employees.GetDriversAsync();
            return drivers.Select(MapToDto);
        }

        public async Task<IEnumerable<EmployeeDto>> GetAdminStaffAsync()
        {
            var staff = await _unitOfWork.Employees.GetAdminStaffAsync();
            return staff.Select(MapToDto);
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesWithExpiredResidencyAsync()
        {
            var employees = await _unitOfWork.Employees.GetEmployeesWithExpiredResidencyAsync();
            return employees.Select(MapToDto);
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesWithExpiringSoonResidencyAsync(int days = 30)
        {
            var employees = await _unitOfWork.Employees.GetEmployeesWithExpiringSoonResidencyAsync(days);
            return employees.Select(MapToDto);
        }

        public async Task<EmployeeDto> CreateEmployeeAsync(EmployeeDto dto, string username)
        {
            var employee = new Employee
            {
                CivilId = dto.CivilId,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                JobTitle = dto.JobTitle,
                Nationality = dto.Nationality,
                IsDriver = dto.IsDriver,
                IsCompanyEmployee = dto.IsCompanyEmployee,
                IsPurchasingDriver = dto.IsPurchasingDriver,
                Salary = dto.Salary,
                ResidencyExpiryDate = dto.ResidencyExpiryDate,
                ResidencyStartDate = dto.ResidencyStartDate,
                BankCardNumber = dto.BankCardNumber,
                BankCardExpiryMonth = dto.BankCardExpiryMonth,
                BankCardExpiryYear = dto.BankCardExpiryYear,
                Notes = dto.Notes,
                CreatedBy = username,
                CreatedDate = DateTime.Now
            };

            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(employee);
        }

        public async Task<EmployeeDto> UpdateEmployeeAsync(EmployeeDto dto, string username)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(dto.EmployeeId);
            if (employee == null)
                throw new KeyNotFoundException($"Employee with ID {dto.EmployeeId} not found");

            employee.CivilId = dto.CivilId;
            employee.FullName = dto.FullName;
            employee.PhoneNumber = dto.PhoneNumber;
            employee.JobTitle = dto.JobTitle;
            employee.Nationality = dto.Nationality;
            employee.IsDriver = dto.IsDriver;
            employee.IsCompanyEmployee = dto.IsCompanyEmployee;
            employee.IsPurchasingDriver = dto.IsPurchasingDriver;
            employee.Salary = dto.Salary;
            employee.ResidencyExpiryDate = dto.ResidencyExpiryDate;
            employee.ResidencyStartDate = dto.ResidencyStartDate;
            employee.BankCardNumber = dto.BankCardNumber;
            employee.BankCardExpiryMonth = dto.BankCardExpiryMonth;
            employee.BankCardExpiryYear = dto.BankCardExpiryYear;
            employee.Notes = dto.Notes;
            employee.UpdatedBy = username;
            employee.UpdatedDate = DateTime.Now;

            _unitOfWork.Employees.Update(employee);
            await _unitOfWork.SaveChangesAsync();

            return MapToDto(employee);
        }

        public async Task DeleteEmployeeAsync(int id, string username)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
                throw new KeyNotFoundException($"Employee with ID {id} not found");

            employee.IsDeleted = true;
            employee.DeletedBy = username;
            employee.DeletedDate = DateTime.Now;

            _unitOfWork.Employees.Update(employee);
            await _unitOfWork.SaveChangesAsync();
        }

        private EmployeeDto MapToDto(Employee employee)
        {
            var currentAssignment = employee.TruckAssignments?.FirstOrDefault(a => a.IsCurrent);

            return new EmployeeDto
            {
                EmployeeId = employee.EmployeeId,
                CivilId = employee.CivilId,
                FullName = employee.FullName,
                PhoneNumber = employee.PhoneNumber,
                JobTitle = employee.JobTitle,
                JobTitleDisplay = employee.JobTitle.ToString().Replace("_", " "),
                Nationality = employee.Nationality,
                NationalityDisplay = employee.Nationality?.ToString().Replace("_", " "),
                IsDriver = employee.IsDriver,
                IsCompanyEmployee = employee.IsCompanyEmployee,
                IsPurchasingDriver = employee.IsPurchasingDriver,
                Salary = employee.Salary,
                ResidencyExpiryDate = employee.ResidencyExpiryDate,
                ResidencyStartDate = employee.ResidencyStartDate,
                BankCardNumber = employee.BankCardNumber,
                BankCardExpiryMonth = employee.BankCardExpiryMonth,
                BankCardExpiryYear = employee.BankCardExpiryYear,
                Notes = employee.Notes,
                CurrentTruckPlate = currentAssignment?.Truck?.PlateNumber,
                CurrentTruckId = currentAssignment?.TruckId,
                IsResidencyExpired = employee.IsResidencyExpired(),
                IsResidencyExpiringSoon = employee.IsResidencyExpiringSoon(),
                IsResidencyWarning = employee.IsResidencyWarning(),
                CreatedBy = employee.CreatedBy,
                CreatedDate = employee.CreatedDate,
                UpdatedBy = employee.UpdatedBy,
                UpdatedDate = employee.UpdatedDate
            };
        }
    }
}
