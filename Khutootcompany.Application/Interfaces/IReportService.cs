using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.Interfaces
{
    public interface IReportService
    {
        Task<byte[]> GeneratePAMReportAsync();
        Task<byte[]> GenerateExpiredResidenciesReportAsync();
        Task<byte[]> GenerateOverdueInstallmentsReportAsync();
        Task<byte[]> GenerateExpiredWakalatReportAsync();
        Task<byte[]> GenerateExpiredInsuranceReportAsync();
        Task<byte[]> GenerateMonthlyCashReportAsync(int year, int month);
    }
}
