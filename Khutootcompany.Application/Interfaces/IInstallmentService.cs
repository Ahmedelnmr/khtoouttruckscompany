using Khutootcompany.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.Interfaces
{
    public interface IInstallmentService
    {
        Task<IEnumerable<InstallmentDto>> GetAllInstallmentsAsync();
        Task<InstallmentDto?> GetInstallmentByIdAsync(int id);
        Task<IEnumerable<InstallmentDto>> GetActiveInstallmentsAsync();
        Task<IEnumerable<InstallmentDto>> GetOverdueInstallmentsAsync();
        Task<InstallmentDto?> GetInstallmentByTruckIdAsync(int truckId);
        Task<IEnumerable<InstallmentDto>> GetInstallmentsByEmployeeIdAsync(int employeeId);
        Task<InstallmentDto> CreateInstallmentAsync(InstallmentDto dto, string username);
        Task<InstallmentDto> UpdateInstallmentAsync(InstallmentDto dto, string username);
        Task RecordPaymentAsync(int installmentId, decimal amount, string paymentType, string? sondNumber, string username);
        Task DeleteInstallmentAsync(int id, string username);
    }
}
