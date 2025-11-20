using Khutootcompany.Application.DTOs;
using Khutootcompany.Application.Interfaces;
using Khutootcompany.Application.Services;
using Khutootcompany.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Khutootcompany.presention.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // GET: Employees
        public async Task<IActionResult> Index(string? filter)
        {
            IEnumerable<EmployeeDto> employees;

            switch (filter)
            {
                case "drivers":
                    employees = await _employeeService.GetDriversAsync();
                    ViewBag.FilterTitle = "السواقين فقط";
                    break;
                case "admin-staff":
                    employees = await _employeeService.GetAdminStaffAsync();
                    ViewBag.FilterTitle = "الموظفين الإداريين";
                    break;
                case "expired-residency":
                    employees = await _employeeService.GetEmployeesWithExpiredResidencyAsync();
                    ViewBag.FilterTitle = "إقامات منتهية";
                    break;
                case "expiring-residency":
                    employees = await _employeeService.GetEmployeesWithExpiringSoonResidencyAsync(30);
                    ViewBag.FilterTitle = "إقامات تنتهي قريباً";
                    break;
                default:
                    employees = await _employeeService.GetAllEmployeesAsync();
                    ViewBag.FilterTitle = "جميع الموظفين";
                    break;
            }

            return View(employees);
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound();

            return View(employee);
        }

        // GET: Employees/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(EmployeeDto employee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var username = User.Identity?.Name ?? "Unknown";
                    await _employeeService.CreateEmployeeAsync(employee, username);
                    TempData["Success"] = "تم إضافة الموظف بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"حدث خطأ: {ex.Message}");
                }
            }

            PopulateDropdowns();
            return View(employee);
        }

        // GET: Employees/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound();

            PopulateDropdowns();
            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, EmployeeDto employee)
        {
            if (id != employee.EmployeeId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var username = User.Identity?.Name ?? "Unknown";
                    await _employeeService.UpdateEmployeeAsync(employee, username);
                    TempData["Success"] = "تم تحديث الموظف بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"حدث خطأ: {ex.Message}");
                }
            }

            PopulateDropdowns();
            return View(employee);
        }

        // GET: Employees/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound();

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var username = User.Identity?.Name ?? "Unknown";
                await _employeeService.DeleteEmployeeAsync(id, username);
                TempData["Success"] = "تم حذف الموظف بنجاح";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"حدث خطأ: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        private void PopulateDropdowns()
        {
            ViewBag.JobTitles = new SelectList(Enum.GetValues(typeof(JobTitle))
                .Cast<JobTitle>()
                .Select(e => new { Value = (int)e, Text = e.ToString().Replace("_", " ") }),
                "Value", "Text");

            ViewBag.Nationalities = new SelectList(Enum.GetValues(typeof(Nationality))
                .Cast<Nationality>()
                .Select(e => new { Value = (int)e, Text = e.ToString().Replace("_", " ") }),
                "Value", "Text");
        }
    }
}
