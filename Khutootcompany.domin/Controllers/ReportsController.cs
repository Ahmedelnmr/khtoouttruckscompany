using Khutootcompany.Application.Interfaces;
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

        [HttpGet]
        public async Task<IActionResult> GeneratePAMReport()
        {
            try
            {
                var pdfBytes = await _reportService.GeneratePAMReportAsync();
                return File(pdfBytes, "application/pdf", $"PAM_Report_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"حدث خطأ أثناء إنشاء التقرير: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GenerateExpiredResidenciesReport()
        {
            try
            {
                var pdfBytes = await _reportService.GenerateExpiredResidenciesReportAsync();
                return File(pdfBytes, "application/pdf", $"Expired_Residencies_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"حدث خطأ أثناء إنشاء التقرير: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GenerateOverdueInstallmentsReport()
        {
            try
            {
                var pdfBytes = await _reportService.GenerateOverdueInstallmentsReportAsync();
                return File(pdfBytes, "application/pdf", $"Overdue_Installments_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"حدث خطأ أثناء إنشاء التقرير: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GenerateExpiredWakalatReport()
        {
            try
            {
                var pdfBytes = await _reportService.GenerateExpiredWakalatReportAsync();
                return File(pdfBytes, "application/pdf", $"Expired_Wakalat_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"حدث خطأ أثناء إنشاء التقرير: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GenerateExpiredInsuranceReport()
        {
            try
            {
                var pdfBytes = await _reportService.GenerateExpiredInsuranceReportAsync();
                return File(pdfBytes, "application/pdf", $"Expired_Insurance_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"حدث خطأ أثناء إنشاء التقرير: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GenerateMonthlyCashReport(int year, int month)
        {
            if (year < 2000 || year > 2100 || month < 1 || month > 12)
            {
                TempData["Error"] = "الرجاء إدخال سنة وشهر صحيحين";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var pdfBytes = await _reportService.GenerateMonthlyCashReportAsync(year, month);
                return File(pdfBytes, "application/pdf", $"Cash_Report_{year}_{month:D2}.pdf");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"حدث خطأ أثناء إنشاء التقرير: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}