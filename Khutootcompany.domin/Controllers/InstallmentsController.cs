using Khutootcompany.Application.DTOs;
using Khutootcompany.Application.Interfaces;
using Khutootcompany.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khutootcompany.presention.Controllers
{
    [Authorize]
    public class InstallmentsController : Controller
    {
        private readonly IInstallmentService _installmentService;
        private readonly ITruckService _truckService;
        private readonly IEmployeeService _employeeService;

        public InstallmentsController(
            IInstallmentService installmentService,
            ITruckService truckService,
            IEmployeeService employeeService)
        {
            _installmentService = installmentService;
            _truckService = truckService;
            _employeeService = employeeService;
        }

        // GET: Installments
        public async Task<IActionResult> Index(string? filter)
        {
            IEnumerable<InstallmentDto> installments;

            switch (filter)
            {
                case "overdue":
                    installments = await _installmentService.GetOverdueInstallmentsAsync();
                    ViewBag.FilterTitle = "أقساط متأخرة";
                    break;
                case "active":
                    installments = await _installmentService.GetActiveInstallmentsAsync();
                    ViewBag.FilterTitle = "أقساط نشطة";
                    break;
                default:
                    installments = await _installmentService.GetAllInstallmentsAsync();
                    ViewBag.FilterTitle = "جميع الأقساط";
                    break;
            }

            return View(installments);
        }

        // GET: Installments/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var installment = await _installmentService.GetInstallmentByIdAsync(id);
            if (installment == null)
                return NotFound();

            return View(installment);
        }

        // GET: Installments/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        // POST: Installments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(InstallmentDto installment)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var username = User.Identity?.Name ?? "Unknown";
                    await _installmentService.CreateInstallmentAsync(installment, username);
                    TempData["Success"] = "تم إضافة القسط بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"حدث خطأ: {ex.Message}");
                }
            }

            await PopulateDropdowns();
            return View(installment);
        }

        // POST: Installments/RecordPayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordPayment(int installmentId, decimal amount, string paymentType, string? sondNumber)
        {
            try
            {
                var username = User.Identity?.Name ?? "Unknown";
                await _installmentService.RecordPaymentAsync(installmentId, amount, paymentType, sondNumber, username);
                TempData["Success"] = "تم تسجيل الدفعة بنجاح";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"حدث خطأ: {ex.Message}";
            }

            return RedirectToAction(nameof(Details), new { id = installmentId });
        }

        private async Task PopulateDropdowns()
        {
            var trucks = await _truckService.GetAllTrucksAsync();
            var employees = await _employeeService.GetDriversAsync();

            ViewBag.Trucks = trucks.Select(t => new {
                Value = t.TruckId,
                Text = $"{t.PlateNumber} - {t.Model}"
            });

            ViewBag.Employees = employees.Select(e => new {
                Value = e.EmployeeId,
                Text = e.FullName
            });
        }
    }
}
