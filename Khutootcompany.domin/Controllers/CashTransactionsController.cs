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
    public class CashTransactionsController : Controller
    {
        private readonly ICashTransactionService _cashService;
        private readonly IEmployeeService _employeeService;
        private readonly ITruckService _truckService;

        public CashTransactionsController(
            ICashTransactionService cashService,
            IEmployeeService employeeService,
            ITruckService truckService)
        {
            _cashService = cashService;
            _employeeService = employeeService;
            _truckService = truckService;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, TransactionType? type)
        {
            IEnumerable<CashTransactionDto> transactions;

            if (startDate.HasValue && endDate.HasValue)
            {
                transactions = await _cashService.GetTransactionsByDateRangeAsync(startDate.Value, endDate.Value);
                ViewBag.FilterTitle = $"من {startDate:dd/MM/yyyy} إلى {endDate:dd/MM/yyyy}";
            }
            else if (type.HasValue)
            {
                transactions = await _cashService.GetTransactionsByTypeAsync(type.Value);
                ViewBag.FilterTitle = type.Value.ToString().Replace("_", " ");
            }
            else
            {
                transactions = await _cashService.GetAllTransactionsAsync();
                ViewBag.FilterTitle = "جميع الحركات";
            }

            ViewBag.CurrentBalance = await _cashService.GetCurrentBalanceAsync();
            return View(transactions);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }
       // GET: /CashTransactions/Edit/5
public async Task<IActionResult> Edit(int id)  // لازم يكون فيه (int id)
{
    var transaction = await _cashService.GetTransactionByIdAsync(id);
    
    if (transaction == null)
        return NotFound();

    // تحويل الكيان إلى DTO
    var dto = new CreateCashTransactionDto
    {
        Id = transaction.TransactionId,
        TransactionDate = transaction.TransactionDate,
        Amount = transaction.Amount,
        Type = transaction.Type,
        EmployeeId = transaction.EmployeeId,
        TruckId = transaction.TruckId,
        Description = transaction.Description,
        SondNumber = transaction.SondNumber,
        Notes = transaction.Notes
    };

    await PopulateDropdowns();
    return View(dto);  // هنا بقى بترجع الـ dto مش null
}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit (int id, CreateCashTransactionDto createCashTransactionDto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(createCashTransactionDto);
            }

            try
            {
                await _cashService.UpdateTransactionAsync(createCashTransactionDto, User.Identity?.Name);
                TempData["Success"] = "تم تحديث الحركة بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"حدث خطأ: {ex.Message}");
            }

            await PopulateDropdowns();
            return View(createCashTransactionDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateCashTransactionDto transaction)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var username = User.Identity?.Name ?? "Unknown";
                    await _cashService.CreateTransactionAsync(transaction, username);
                    TempData["Success"] = "تم تسجيل الحركة بنجاح";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"حدث خطأ: {ex.Message}");
                }
            }

            await PopulateDropdowns();
            return View(transaction);
        }

        private async Task PopulateDropdowns()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            var trucks = await _truckService.GetAllTrucksAsync();

            ViewBag.Employees = employees.Select(e => new { Value = e.EmployeeId, Text = e.FullName });
            ViewBag.Trucks = trucks.Select(t => new { Value = t.TruckId, Text = t.PlateNumber });
            ViewBag.TransactionTypes = new SelectList(Enum.GetValues(typeof(TransactionType))
                .Cast<TransactionType>()
                .Select(e => new { Value = (int)e, Text = e.ToString().Replace("_", " ") }),
                "Value", "Text");
        }
        public async Task <IActionResult> delete (int id)
        {
            var transaction = await _cashService.GetTransactionByIdAsync(id);
            if (transaction == null)
                return NotFound();

            return View(transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _cashService.DeleteTransactionAsync(id, User.Identity?.Name);
                TempData["Success"] = "تم حذف الحركة بنجاح";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"حدث خطأ أثناء الحذف: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
