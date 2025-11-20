using Khutootcompany.Application.Interfaces;
using Khutootcompany.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khutootcompany.presention.Controllers
{

    [Authorize]
    public class InsuranceRenewalsController : Controller
    {
        private readonly IInsuranceRenewalService _insuranceService;
        private readonly ITruckService _truckService;

        public InsuranceRenewalsController(
            IInsuranceRenewalService insuranceService,
            ITruckService truckService)
        {
            _insuranceService = insuranceService;
            _truckService = truckService;
        }

        public async Task<IActionResult> Index()
        {
            var expiredTrucks = await _truckService.GetTrucksWithExpiredInsuranceAsync();
            var expiringSoon = await _truckService.GetTrucksWithExpiringSoonInsuranceAsync(30);

            ViewBag.ExpiredTrucks = expiredTrucks;
            ViewBag.ExpiringSoonTrucks = expiringSoon;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Renew(int truckId, DateTime expiryDate, decimal cost, string? sondNumber)
        {
            try
            {
                var username = User.Identity?.Name ?? "Unknown";
                await _insuranceService.RenewInsuranceAsync(truckId, expiryDate, cost, sondNumber, username);
                TempData["Success"] = "تم تجديد التأمين بنجاح";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"حدث خطأ: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
