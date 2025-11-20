using Khutootcompany.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.Interfaces
{

    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
        Task<EmployeeDto?> GetEmployeeByIdAsync(int id);
        Task<EmployeeDto?> GetEmployeeByCivilIdAsync(string civilId);
        Task<IEnumerable<EmployeeDto>> GetDriversAsync();
        Task<IEnumerable<EmployeeDto>> GetAdminStaffAsync();
        Task<IEnumerable<EmployeeDto>> GetEmployeesWithExpiredResidencyAsync();
        Task<IEnumerable<EmployeeDto>> GetEmployeesWithExpiringSoonResidencyAsync(int days = 30);
        Task<EmployeeDto> CreateEmployeeAsync(EmployeeDto dto, string username);
        Task<EmployeeDto> UpdateEmployeeAsync(EmployeeDto dto, string username);
        Task DeleteEmployeeAsync(int id, string username);
    }
}
