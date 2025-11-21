using Khutootcompany.Application.DTOs;
using Khutootcompany.Application.Interfaces;
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
                case "completed":
                    var allInstallments = await _installmentService.GetAllInstallmentsAsync();
                    installments = allInstallments.Where(i => i.IsCompleted);
                    ViewBag.FilterTitle = "أقساط مكتملة";
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
            return View(new InstallmentDto());
        }

        // POST: Installments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(InstallmentDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if truck already has an active installment
                    var existingInstallment = await _installmentService.GetInstallmentByTruckIdAsync(dto.TruckId);
                    if (existingInstallment != null && !existingInstallment.IsCompleted)
                    {
                        ModelState.AddModelError("", "هذه الشاحنة لديها قسط نشط بالفعل");
                        await PopulateDropdowns();
                        return View(dto);
                    }

                    var username = User.Identity?.Name ?? "Unknown";

                    var installment = new InstallmentDto
                    {
                        TruckId = dto.TruckId,
                        EmployeeId = dto.EmployeeId,
                        TotalPriceOffice = dto.TotalPriceOffice,
                        TotalPriceSayer = dto.TotalPriceSayer,
                        MonthlyQest = dto.MonthlyQest,
                        FinanceSource = dto.FinanceSource,
                        SayerTransactionNumber = dto.SayerTransactionNumber,
                        StartDate = dto.StartDate,
                        TotalMonths = dto.TotalMonths,
                        Notes = dto.Notes,
                        CreatedBy = username,
                        CreatedDate= DateTime.Now

                    };

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
            return View(dto);
        }

        // GET: Installments/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var installment = await _installmentService.GetInstallmentByIdAsync(id);
            if (installment == null)
                return NotFound();

            await PopulateDropdowns();
            return View(installment);
        }

        // POST: Installments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, InstallmentDto installment)
        {
            if (id != installment.InstallmentId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var username = User.Identity?.Name ?? "Unknown";
                    await _installmentService.UpdateInstallmentAsync(installment, username);
                    TempData["Success"] = "تم تحديث القسط بنجاح";
                    return RedirectToAction(nameof(Details), new { id = installment.InstallmentId });
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

            await PopulateDropdowns();
            return View(installment);
        }

        // POST: Installments/RecordPayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordPayment(RecordPaymentDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var username = User.Identity?.Name ?? "Unknown";
                    await _installmentService.RecordPaymentAsync(
                        dto.InstallmentId,
                        dto.Amount,
                        dto.PaymentType,
                        dto.SondNumber,
                        username);

                    TempData["Success"] = "تم تسجيل الدفعة بنجاح";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"حدث خطأ: {ex.Message}";
                }
            }
            else
            {
                TempData["Error"] = "بيانات غير صحيحة";
            }

            return RedirectToAction(nameof(Details), new { id = dto.InstallmentId });
        }

        // GET: Installments/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var installment = await _installmentService.GetInstallmentByIdAsync(id);
            if (installment == null)
                return NotFound();

            return View(installment);
        }

        // POST: Installments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var username = User.Identity?.Name ?? "Unknown";
                await _installmentService.DeleteInstallmentAsync(id, username);
                TempData["Success"] = "تم حذف القسط بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"حدث خطأ: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // GET: Installments/ByEmployee/5
        public async Task<IActionResult> ByEmployee(int employeeId)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
            if (employee == null)
                return NotFound();

            var installments = await _installmentService.GetInstallmentsByEmployeeIdAsync(employeeId);
            ViewBag.EmployeeName = employee.FullName;
            ViewBag.EmployeeId = employeeId;

            return View("Index", installments);
        }

        // GET: Installments/ByTruck/5
        public async Task<IActionResult> ByTruck(int truckId)
        {
            var truck = await _truckService.GetTruckByIdAsync(truckId);
            if (truck == null)
                return NotFound();

            var installment = await _installmentService.GetInstallmentByTruckIdAsync(truckId);
            if (installment == null)
            {
                TempData["Error"] = "لا يوجد قسط نشط لهذه الشاحنة";
                return RedirectToAction("Details", "Trucks", new { id = truckId });
            }

            return RedirectToAction(nameof(Details), new { id = installment.InstallmentId });
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

            ViewBag.FinanceSources = new[] {
                "الساير",
                "أنوار السالمي",
                "السلام",
                "مباشر",
                "البنك الأهلي",
                "بنك الكويت الوطني",
                "أخرى"
            };
        }
    }
}