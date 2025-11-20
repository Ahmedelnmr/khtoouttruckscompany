using Khutootcompany.Application.DTOs;
using Khutootcompany.Application.Interfaces;
using Khutootcompany.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khutootcompany.presention.Controllers
{
    [Authorize]
    public class WakalatController : Controller
    {
        private readonly IWakalaService _wakalaService;
        private readonly IEmployeeService _employeeService;
        private readonly ITruckService _truckService;

        public WakalatController(
            IWakalaService wakalaService,
            IEmployeeService employeeService,
            ITruckService truckService)
        {
            _wakalaService = wakalaService;
            _employeeService = employeeService;
            _truckService = truckService;
        }

        public async Task<IActionResult> Index(string? filter)
        {
            IEnumerable<WakalaDto> wakalat;

            switch (filter)
            {
                case "expired":
                    wakalat = await _wakalaService.GetExpiredWakalatAsync();
                    ViewBag.FilterTitle = "وكالات منتهية";
                    break;
                case "expiring":
                    wakalat = await _wakalaService.GetExpiringSoonWakalatAsync(30);
                    ViewBag.FilterTitle = "وكالات تنتهي قريباً";
                    break;
                case "general":
                    wakalat = await _wakalaService.GetGeneralWakalatAsync();
                    ViewBag.FilterTitle = "وكالات عامة";
                    break;
                default:
                    wakalat = await _wakalaService.GetAllWakalatAsync();
                    ViewBag.FilterTitle = "جميع الوكالات";
                    break;
            }

            return View(wakalat);
        }

        public async Task<IActionResult> Details(int id)
        {
            var wakala = await _wakalaService.GetWakalaByIdAsync(id);
            if (wakala == null)
                return NotFound();

            return View(wakala);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(WakalaDto wakala)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var username = User.Identity?.Name ?? "Unknown";
                    await _wakalaService.CreateWakalaAsync(wakala, username);
                    TempData["Success"] = "تم إضافة الوكالة بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"حدث خطأ: {ex.Message}");
                }
            }

            await PopulateDropdowns();
            return View(wakala);
        }

        private async Task PopulateDropdowns()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            var trucks = await _truckService.GetAllTrucksAsync();

            ViewBag.Employees = employees.Select(e => new { Value = e.EmployeeId, Text = e.FullName });
            ViewBag.Trucks = trucks.Select(t => new { Value = t.TruckId, Text = t.PlateNumber });
        }
    }
}
