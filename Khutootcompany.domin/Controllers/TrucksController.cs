using Khutootcompany.Application.DTOs;
using Khutootcompany.Application.Interfaces;
using Khutootcompany.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Khutootcompany.presention.Controllers
{
    [Authorize]
    public class TrucksController : Controller
    {
        private readonly ITruckService _truckService;
        private readonly IEmployeeService _employeeService;

        public TrucksController(
            ITruckService truckService,
            IEmployeeService employeeService)
        {
            _truckService = truckService;
            _employeeService = employeeService;
        }

        // GET: Trucks
        public async Task<IActionResult> Index(string? filter)
        {
            IEnumerable<TruckDto> trucks;

            switch (filter)
            {
                case "expired-insurance":
                    trucks = await _truckService.GetTrucksWithExpiredInsuranceAsync();
                    ViewBag.FilterTitle = "شاحنات بتأمين منتهي";
                    break;
                case "expiring-insurance":
                    trucks = await _truckService.GetTrucksWithExpiringSoonInsuranceAsync(30);
                    ViewBag.FilterTitle = "شاحنات التأمين ينتهي خلال شهر";
                    break;
                case "not-accepted-pam":
                    trucks = await _truckService.GetTrucksByPAMStatusAsync(PAMStatus.غير_مقبول);
                    ViewBag.FilterTitle = "شاحنات غير مقبولة في PAM";
                    break;
                case "general-transport":
                    trucks = await _truckService.GetTrucksByLicenseTypeAsync(LicenseType.نقل_عام);
                    ViewBag.FilterTitle = "شاحنات نقل عام";
                    break;
                case "external-goods":
                    trucks = await _truckService.GetTrucksByLicenseTypeAsync(LicenseType.بضائع_خارجي);
                    ViewBag.FilterTitle = "شاحنات بضائع خارجي";
                    break;
                default:
                    trucks = await _truckService.GetAllTrucksAsync();
                    ViewBag.FilterTitle = "جميع الشاحنات";
                    break;
            }

            return View(trucks);
        }

        // GET: Trucks/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var truck = await _truckService.GetTruckByIdAsync(id);
            if (truck == null)
                return NotFound();

            return View(truck);
        }

        // GET: Trucks/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }

        // POST: Trucks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(TruckDto truck)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var username = User.Identity?.Name ?? "Unknown";
                    await _truckService.CreateTruckAsync(truck, username);
                    TempData["Success"] = "تم إضافة الشاحنة بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"حدث خطأ: {ex.Message}");
                }
            }

            PopulateDropdowns();
            return View(truck);
        }

        // GET: Trucks/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var truck = await _truckService.GetTruckByIdAsync(id);
            if (truck == null)
                return NotFound();

            PopulateDropdowns();
            return View(truck);
        }

        // POST: Trucks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, TruckDto truck)
        {
            if (id != truck.TruckId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var username = User.Identity?.Name ?? "Unknown";
                    await _truckService.UpdateTruckAsync(truck, username);
                    TempData["Success"] = "تم تحديث الشاحنة بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"حدث خطأ: {ex.Message}");
                }
            }

            PopulateDropdowns();
            return View(truck);
        }

        // GET: Trucks/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var truck = await _truckService.GetTruckByIdAsync(id);
            if (truck == null)
                return NotFound();

            return View(truck);
        }

        // POST: Trucks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var username = User.Identity?.Name ?? "Unknown";
                await _truckService.DeleteTruckAsync(id, username);
                TempData["Success"] = "تم حذف الشاحنة بنجاح";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"حدث خطأ: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Trucks/AssignDriver
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignDriver(int truckId, int employeeId)
        {
            try
            {
                var username = User.Identity?.Name ?? "Unknown";
                await _truckService.AssignDriverAsync(truckId, employeeId, username);
                TempData["Success"] = "تم تعيين السائق بنجاح";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"حدث خطأ: {ex.Message}";
            }

            return RedirectToAction(nameof(Details), new { id = truckId });
        }

        private void PopulateDropdowns()
        {
            ViewBag.LicenseTypes = new SelectList(
                Enum.GetValues(typeof(LicenseType))
                    .Cast<LicenseType>()
                    .Select(e => new { Value = (int)e, Text = e.ToString().Replace("_", " ") }),
                "Value", "Text");

            ViewBag.PAMStatuses = new SelectList(
                Enum.GetValues(typeof(PAMStatus))
                    .Cast<PAMStatus>()
                    .Select(e => new { Value = (int)e, Text = e.ToString().Replace("_", " ") }),
                "Value", "Text");
        }
    }
}
