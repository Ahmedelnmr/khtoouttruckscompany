using Khutootcompany.Application.DTOs;
using Khutootcompany.Application.Interfaces;
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

        // GET: VisitCards
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
                case "valid":
                    var allCards = await _visitCardService.GetAllVisitCardsAsync();
                    cards = allCards.Where(c => !c.IsExpired && !c.IsExpiringSoon);
                    ViewBag.FilterTitle = "كروت صالحة";
                    break;
                case "unpaid":
                    var unpaidCards = await _visitCardService.GetAllVisitCardsAsync();
                    cards = unpaidCards.Where(c => !c.IsPaid);
                    ViewBag.FilterTitle = "كروت غير مدفوعة";
                    break;
                default:
                    cards = await _visitCardService.GetAllVisitCardsAsync();
                    ViewBag.FilterTitle = "جميع كروت الزيارة";
                    break;
            }

            return View(cards);
        }

        // GET: VisitCards/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var card = await _visitCardService.GetVisitCardByIdAsync(id);
            if (card == null)
                return NotFound();

            return View(card);
        }

        // GET: VisitCards/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        // POST: VisitCards/Create
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

        // GET: VisitCards/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var card = await _visitCardService.GetVisitCardByIdAsync(id);
            if (card == null)
                return NotFound();

            await PopulateDropdowns();
            return View(card);
        }

        // POST: VisitCards/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, VisitCardDto card)
        {
            if (id != card.VisitCardId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var username = User.Identity?.Name ?? "Unknown";
                    await _visitCardService.UpdateVisitCardAsync(card, username);
                    TempData["Success"] = "تم تحديث كارت الزيارة بنجاح";
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

            await PopulateDropdowns();
            return View(card);
        }

        // GET: VisitCards/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var card = await _visitCardService.GetVisitCardByIdAsync(id);
            if (card == null)
                return NotFound();

            return View(card);
        }

        // POST: VisitCards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var username = User.Identity?.Name ?? "Unknown";
                await _visitCardService.DeleteVisitCardAsync(id, username);
                TempData["Success"] = "تم حذف كارت الزيارة بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"حدث خطأ: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: VisitCards/ByEmployee/5
        public async Task<IActionResult> ByEmployee(int employeeId)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
            if (employee == null)
                return NotFound();

            var cards = await _visitCardService.GetVisitCardsByEmployeeAsync(employeeId);
            ViewBag.EmployeeName = employee.FullName;
            ViewBag.EmployeeId = employeeId;
            ViewBag.FilterTitle = $"كروت زيارة {employee.FullName}";

            return View("Index", cards);
        }

        // GET: VisitCards/Renew/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Renew(int id)
        {
            var oldCard = await _visitCardService.GetVisitCardByIdAsync(id);
            if (oldCard == null)
                return NotFound();

            var newCard = new CreateVisitCardDto
            {
                EmployeeId = oldCard.EmployeeId,
                TruckId = oldCard.TruckId,
                IssueDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddMonths(3),
                Price = 15m,
                IsPaid = false,
                IntermediaryName = oldCard.IntermediaryName,
                IntermediaryPhone = oldCard.IntermediaryPhone
            };

            await PopulateDropdowns();
            ViewBag.OldCard = oldCard;
            ViewBag.IsRenewal = true;

            return View("Create", newCard);
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