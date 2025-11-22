using Khutootcompany.Application.DTOs;
using Khutootcompany.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        // ⭐ GET: /Wakalat
        public async Task<IActionResult> Index(string? filter)
        {
            IEnumerable<WakalaDto> wakalat;

            switch (filter?.ToLower())
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
                case "truck-specific":
                    var all = await _wakalaService.GetAllWakalatAsync();
                    wakalat = all.Where(w => !w.IsGeneral && w.TruckId.HasValue);
                    ViewBag.FilterTitle = "وكالات على شاحنات";
                    break;
                case "valid":
                    var allWakalat = await _wakalaService.GetAllWakalatAsync();
                    wakalat = allWakalat.Where(w => !w.IsExpired);
                    ViewBag.FilterTitle = "وكالات صالحة";
                    break;
                case "unpaid":
                    var allW = await _wakalaService.GetAllWakalatAsync();
                    wakalat = allW.Where(w => !w.IsPaid);
                    ViewBag.FilterTitle = "وكالات غير مدفوعة";
                    break;
                default:
                    wakalat = await _wakalaService.GetAllWakalatAsync();
                    ViewBag.FilterTitle = "جميع الوكالات";
                    break;
            }

            return View(wakalat);
        }

        // ⭐ GET: /Wakalat/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var wakala = await _wakalaService.GetWakalaByIdAsync(id);
            if (wakala == null)
                return NotFound();

            return View(wakala);
        }

        // ⭐ GET: /Wakalat/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View(new WakalaDto { IssueDate = DateTime.Now });
        }

        // ⭐ POST: /Wakalat/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(WakalaDto wakala)
        {
            // Custom validation
            if (!wakala.IsGeneral && !wakala.TruckId.HasValue)
            {
                ModelState.AddModelError("TruckId", "يجب تحديد الشاحنة للوكالة غير العامة");
            }

            if (wakala.ExpiryDate <= wakala.IssueDate)
            {
                ModelState.AddModelError("ExpiryDate", "تاريخ الانتهاء يجب أن يكون بعد تاريخ الإصدار");
            }

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

        // ⭐ GET: /Wakalat/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var wakala = await _wakalaService.GetWakalaByIdAsync(id);
            if (wakala == null)
                return NotFound();

            await PopulateDropdowns();
            return View(wakala);
        }

        // ⭐ POST: /Wakalat/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, WakalaDto wakala)
        {
            if (id != wakala.WakalaId)
                return NotFound();

            // Custom validation
            if (!wakala.IsGeneral && !wakala.TruckId.HasValue)
            {
                ModelState.AddModelError("TruckId", "يجب تحديد الشاحنة للوكالة غير العامة");
            }

            if (wakala.ExpiryDate <= wakala.IssueDate)
            {
                ModelState.AddModelError("ExpiryDate", "تاريخ الانتهاء يجب أن يكون بعد تاريخ الإصدار");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var username = User.Identity?.Name ?? "Unknown";
                    await _wakalaService.UpdateWakalaAsync(wakala, username);
                    TempData["Success"] = "تم تحديث الوكالة بنجاح";
                    return RedirectToAction(nameof(Details), new { id = wakala.WakalaId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"حدث خطأ: {ex.Message}");
                }
            }

            await PopulateDropdowns();
            return View(wakala);
        }

        // ⭐ GET: /Wakalat/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var wakala = await _wakalaService.GetWakalaByIdAsync(id);
            if (wakala == null)
                return NotFound();

            return View(wakala);
        }

        // ⭐ POST: /Wakalat/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var username = User.Identity?.Name ?? "Unknown";
                await _wakalaService.DeleteWakalaAsync(id, username);
                TempData["Success"] = "تم حذف الوكالة بنجاح";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"حدث خطأ: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // ⭐ GET: /Wakalat/ByEmployee/5
        public async Task<IActionResult> ByEmployee(int employeeId)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
            if (employee == null)
                return NotFound();

            var wakalat = await _wakalaService.GetWakalatByEmployeeAsync(employeeId);

            ViewBag.EmployeeName = employee.FullName;
            ViewBag.EmployeeId = employeeId;
            ViewBag.FilterTitle = $"وكالات الموظف: {employee.FullName}";

            return View("Index", wakalat);
        }

        // ⭐ GET: /Wakalat/ByTruck/5
        public async Task<IActionResult> ByTruck(int truckId)
        {
            var truck = await _truckService.GetTruckByIdAsync(truckId);
            if (truck == null)
                return NotFound();

            var wakalat = await _wakalaService.GetWakalatByTruckAsync(truckId);

            ViewBag.TruckPlate = truck.PlateNumber;
            ViewBag.TruckId = truckId;
            ViewBag.FilterTitle = $"وكالات الشاحنة: {truck.PlateNumber}";

            return View("Index", wakalat);
        }

        // ⭐ Helper: Populate Dropdowns
        private async Task PopulateDropdowns()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            var trucks = await _truckService.GetAllTrucksAsync();

            ViewBag.Employees = employees
                .OrderBy(e => e.FullName)
                .Select(e => new { Value = e.EmployeeId, Text = e.FullName });

            ViewBag.Trucks = trucks
                .OrderBy(t => t.PlateNumber)
                .Select(t => new { Value = t.TruckId, Text = $"{t.PlateNumber} - {t.Model}" });
        }
    }
}