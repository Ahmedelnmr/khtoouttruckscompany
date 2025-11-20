using Khutootcompany.Application.DTOs;
using Khutootcompany.Application.Interfaces;
using Khutootcompany.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khutootcompany.presention.Controllers
{

    [Authorize]
    public class VisitCardsController : Controller
    {
        private readonly IVisitCardService _visitCardService;
        private readonly IEmployeeService _employeeService;
        private readonly ITruckService _truckService;

        public VisitCardsController(
            IVisitCardService visitCardService,
            IEmployeeService employeeService,
            ITruckService truckService)
        {
            _visitCardService = visitCardService;
            _employeeService = employeeService;
            _truckService = truckService;
        }

        public async Task<IActionResult> Index(string? filter)
        {
            IEnumerable<VisitCardDto> cards;

            switch (filter)
            {
                case "expired":
                    cards = await _visitCardService.GetExpiredVisitCardsAsync();
                    ViewBag.FilterTitle = "كروت منتهية";
                    break;
                case "expiring":
                    cards = await _visitCardService.GetExpiringSoonVisitCardsAsync(30);
                    ViewBag.FilterTitle = "كروت تنتهي قريباً";
                    break;
                default:
                    cards = await _visitCardService.GetAllVisitCardsAsync();
                    ViewBag.FilterTitle = "جميع كروت الزيارة";
                    break;
            }

            return View(cards);
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
        public async Task<IActionResult> Create(CreateVisitCardDto card)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var username = User.Identity?.Name ?? "Unknown";
                    await _visitCardService.CreateVisitCardAsync(card, username);
                    TempData["Success"] = "تم إضافة كارت الزيارة بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"حدث خطأ: {ex.Message}");
                }
            }

            await PopulateDropdowns();
            return View(card);
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
