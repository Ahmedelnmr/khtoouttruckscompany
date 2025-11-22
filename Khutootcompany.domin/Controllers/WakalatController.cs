using Khutootcompany.Application.DTOs;
using Khutootcompany.Application.Interfaces;
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

        // GET: Wakalat
        public async Task<IActionResult> Index(string? filter)
        {
            IEnumerable<WakalaDto> wakalat;

            switch (filter?.ToLower())
            {
                case "expired":
                    wakalat = await _wakalaService.GetExpiredWakalatAsync();
                    ViewBag.FilterTitle = "وكالات منتهية";
                    ViewBag.CurrentFilter = "expired";
                    break;
                case "expiring":
                    wakalat = await _wakalaService.GetExpiringSoonWakalatAsync(30);
                    ViewBag.FilterTitle = "وكالات تنتهي قريباً";
                    ViewBag.CurrentFilter = "expiring";
                    break;
                case "general":
                    wakalat = await _wakalaService.GetGeneralWakalatAsync();
                    ViewBag.FilterTitle = "وكالات عامة";
                    ViewBag.CurrentFilter = "general";
                    break;
                default:
                    wakalat = await _wakalaService.GetAllWakalatAsync();
                    ViewBag.FilterTitle = "جميع الوكالات";
                    ViewBag.CurrentFilter = "all";
                    break;
            }

            return View(wakalat);
        }

        // GET: Wakalat/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var wakala = await _wakalaService.GetWakalaByIdAsync(id);
            if (wakala == null)
                return NotFound();

            return View(wakala);
        }

        // GET: Wakalat/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            var model = new CreateWakalaDto
            {
                IssueDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddYears(1)
            };
            return View(model);
        }

        // POST: Wakalat/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateWakalaDto model)
        {
            // Additional validation
            if (model.ExpiryDate <= model.IssueDate)
            {
                ModelState.AddModelError("ExpiryDate", "تاريخ الانتهاء يجب أن يكون بعد تاريخ الإصدار");
            }

            if (!model.IsGeneral && !model.TruckId.HasValue)
            {
                ModelState.AddModelError("TruckId", "يجب اختيار شاحنة أو تحديد وكالة عامة");
            }

            if (model.IsGeneral && model.TruckId.HasValue)
            {
                ModelState.AddModelError("IsGeneral", "لا يمكن تحديد شاحنة مع وكالة عامة");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var username = User.Identity?.Name ?? "Unknown";

                    // Map CreateWakalaDto to WakalaDto
                    var wakalaDto = new WakalaDto
                    {
                        EmployeeId = model.EmployeeId,
                        TruckId = model.TruckId,
                        IssueDate = model.IssueDate,
                        ExpiryDate = model.ExpiryDate,
                        IsGeneral = model.IsGeneral,
                        IsPaid = model.IsPaid,
                        Price = model.Price,
                        SondNumber = model.SondNumber,
                        Notes = model.Notes
                    };

                    await _wakalaService.CreateWakalaAsync(wakalaDto, username);
                    TempData["Success"] = "تم إضافة الوكالة بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"حدث خطأ: {ex.Message}");
                }
            }

            await PopulateDropdowns(model.EmployeeId, model.TruckId);
            return View(model);
        }

        // GET: Wakalat/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var wakala = await _wakalaService.GetWakalaByIdAsync(id);
            if (wakala == null)
                return NotFound();

            await PopulateDropdowns(wakala.EmployeeId, wakala.TruckId);
            return View(wakala);
        }

        // POST: Wakalat/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, WakalaDto model)
        {
            if (id != model.WakalaId)
                return NotFound();

            // Additional validation
            if (model.ExpiryDate <= model.IssueDate)
            {
                ModelState.AddModelError("ExpiryDate", "تاريخ الانتهاء يجب أن يكون بعد تاريخ الإصدار");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var username = User.Identity?.Name ?? "Unknown";
                    await _wakalaService.UpdateWakalaAsync(model, username);
                    TempData["Success"] = "تم تعديل الوكالة بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"حدث خطأ: {ex.Message}");
                }
            }

            await PopulateDropdowns(model.EmployeeId, model.TruckId);
            return View(model);
        }

        // POST: Wakalat/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var username = User.Identity?.Name ?? "Unknown";
                await _wakalaService.DeleteWakalaAsync(id, username);
                TempData["Success"] = "تم حذف الوكالة بنجاح";
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "الوكالة غير موجودة";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"حدث خطأ: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Wakalat/Print/5
        public async Task<IActionResult> Print(int id)
        {
            var wakala = await _wakalaService.GetWakalaByIdAsync(id);
            if (wakala == null)
                return NotFound();

            return View(wakala);
        }

        // GET: Wakalat/ByEmployee/5
        public async Task<IActionResult> ByEmployee(int employeeId)
        {
            var wakalat = await _wakalaService.GetWakalatByEmployeeAsync(employeeId);
            var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);

            ViewBag.EmployeeName = employee?.FullName ?? "غير معروف";
            return View("Index", wakalat);
        }

        // GET: Wakalat/ByTruck/5
        public async Task<IActionResult> ByTruck(int truckId)
        {
            var wakalat = await _wakalaService.GetWakalatByTruckAsync(truckId);
            var truck = await _truckService.GetTruckByIdAsync(truckId);

            ViewBag.TruckPlate = truck?.PlateNumber ?? "غير معروف";
            return View("Index", wakalat);
        }

        // Helper method to populate dropdowns
        private async Task PopulateDropdowns(int? selectedEmployeeId = null, int? selectedTruckId = null)
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            var trucks = await _truckService.GetAllTrucksAsync();

            ViewBag.Employees = employees.Select(e => new
            {
                Value = e.EmployeeId,
                Text = e.FullName,
                Selected = e.EmployeeId == selectedEmployeeId
            });

            ViewBag.Trucks = trucks.Select(t => new
            {
                Value = t.TruckId,
                Text = t.PlateNumber,
                Selected = t.TruckId == selectedTruckId
            });
        }
    }
}