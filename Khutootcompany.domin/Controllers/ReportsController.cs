using Khutootcompany.Application.Interfaces;
using Khutootcompany.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khutootcompany.presention.Controllers
{

    [Authorize]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GeneratePAMReport()
        {
            var pdfBytes = await _reportService.GeneratePAMReportAsync();
            return File(pdfBytes, "application/pdf", $"PAM_Report_{DateTime.Now:yyyyMMdd}.pdf");
        }

        public async Task<IActionResult> GenerateExpiredResidenciesReport()
        {
            var pdfBytes = await _reportService.GenerateExpiredResidenciesReportAsync();
            return File(pdfBytes, "application/pdf", $"Expired_Residencies_{DateTime.Now:yyyyMMdd}.pdf");
        }

        public async Task<IActionResult> GenerateOverdueInstallmentsReport()
        {
            var pdfBytes = await _reportService.GenerateOverdueInstallmentsReportAsync();
            return File(pdfBytes, "application/pdf", $"Overdue_Installments_{DateTime.Now:yyyyMMdd}.pdf");
        }

        public async Task<IActionResult> GenerateMonthlyCashReport(int year, int month)
        {
            var pdfBytes = await _reportService.GenerateMonthlyCashReportAsync(year, month);
            return File(pdfBytes, "application/pdf", $"Cash_Report_{year}_{month:D2}.pdf");
        }
    }
}
