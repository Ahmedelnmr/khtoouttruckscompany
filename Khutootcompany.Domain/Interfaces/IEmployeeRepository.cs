using Khutootcompany.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Domain.Interfaces
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<IEnumerable<Employee>> GetDriversAsync();
        Task<IEnumerable<Employee>> GetAdminStaffAsync();
        Task<IEnumerable<Employee>> GetEmployeesWithExpiredResidencyAsync();
        Task<IEnumerable<Employee>> GetEmployeesWithExpiringSoonResidencyAsync(int daysThreshold = 30);
        Task<Employee?> GetEmployeeWithDetailsAsync(int employeeId);
        Task<Employee?> GetByCivilIdAsync(string civilId);
        Task<Employee?> GetCurrentDriverForTruckAsync(int truckId);
    }
}
