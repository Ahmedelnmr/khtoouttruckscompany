using Khutootcompany.Application.DTOs;
using Khutootcompany.Application.Interfaces;
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

        // GET: InsuranceRenewals
        public async Task<IActionResult> Index()
        {
            var expiredTrucks = await _truckService.GetTrucksWithExpiredInsuranceAsync();
            var expiringSoon = await _truckService.GetTrucksWithExpiringSoonInsuranceAsync(30);
            var allTrucks = await _truckService.GetAllTrucksAsync();

            ViewBag.ExpiredTrucks = expiredTrucks;
            ViewBag.ExpiringSoonTrucks = expiringSoon;
            ViewBag.TotalTrucks = allTrucks.Count();

            return View();
        }

        // GET: InsuranceRenewals/History
        public async Task<IActionResult> History()
        {
            var allTrucks = await _truckService.GetAllTrucksAsync();
            var renewalsHistory = new List<InsuranceRenewalDto>();

            foreach (var truck in allTrucks)
            {
                var renewals = await _insuranceService.GetRenewalsByTruckAsync(truck.TruckId);
                foreach (var renewal in renewals)
                {
                    renewalsHistory.Add(new InsuranceRenewalDto
                    {
                        InsuranceId = renewal.InsuranceId,
                        TruckId = renewal.TruckId,
                        TruckPlate = truck.PlateNumber,
                        RenewalDate = renewal.RenewalDate,
                        ExpiryDate = renewal.ExpiryDate,
                        Cost = renewal.Cost,
                        SondNumber = renewal.SondNumber,
                        Notes = renewal.Notes,
                        CreatedBy = renewal.CreatedBy,
                        CreatedDate = renewal.CreatedDate
                    });
                }
            }

            return View(renewalsHistory.OrderByDescending(r => r.RenewalDate));
        }

        // GET: InsuranceRenewals/ByTruck/5
        public async Task<IActionResult> ByTruck(int truckId)
        {
            var truck = await _truckService.GetTruckByIdAsync(truckId);
            if (truck == null)
                return NotFound();

            var renewals = await _insuranceService.GetRenewalsByTruckAsync(truckId);

            ViewBag.TruckPlate = truck.PlateNumber;
            ViewBag.TruckModel = truck.Model;
            ViewBag.TruckId = truckId;
            ViewBag.CurrentExpiryDate = truck.InsuranceExpiryDate;

            var renewalDtos = renewals.Select(r => new InsuranceRenewalDto
            {
                InsuranceId = r.InsuranceId,
                TruckId = r.TruckId,
                TruckPlate = truck.PlateNumber,
                RenewalDate = r.RenewalDate,
                ExpiryDate = r.ExpiryDate,
                Cost = r.Cost,
                SondNumber = r.SondNumber,
                Notes = r.Notes,
                CreatedBy = r.CreatedBy,
                CreatedDate = r.CreatedDate
            }).OrderByDescending(r => r.RenewalDate);

            return View(renewalDtos);
        }

        // POST: InsuranceRenewals/Renew
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Renew(RenewInsuranceDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Validate expiry date
                    if (dto.ExpiryDate <= DateTime.Today)
                    {
                        TempData["Error"] = "تاريخ الانتهاء يجب أن يكون في المستقبل";
                        return RedirectToAction(nameof(Index));
                    }

                    var username = User.Identity?.Name ?? "Unknown";
                    await _insuranceService.RenewInsuranceAsync(
                        dto.TruckId,
                        dto.ExpiryDate,
                        dto.Cost,
                        dto.SondNumber,
                        username);

                    TempData["Success"] = "تم تجديد التأمين بنجاح";
                    return RedirectToAction("Details", "Trucks", new { id = dto.TruckId });
                }
                catch (KeyNotFoundException ex)
                {
                    TempData["Error"] = $"خطأ: {ex.Message}";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"حدث خطأ أثناء تجديد التأمين: {ex.Message}";
                }
            }
            else
            {
                TempData["Error"] = "بيانات غير صحيحة. الرجاء التحقق من المعلومات المدخلة.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: InsuranceRenewals/RenewForm/5
        public async Task<IActionResult> RenewForm(int truckId)
        {
            var truck = await _truckService.GetTruckByIdAsync(truckId);
            if (truck == null)
                return NotFound();

            var dto = new RenewInsuranceDto
            {
                TruckId = truckId,
                ExpiryDate = DateTime.Today.AddYears(1)
            };

            ViewBag.TruckPlate = truck.PlateNumber;
            ViewBag.TruckModel = truck.Model;
            ViewBag.CurrentExpiryDate = truck.InsuranceExpiryDate;

            return View(dto);
        }

        // POST: InsuranceRenewals/BulkRenew
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BulkRenew(List<int> truckIds, DateTime expiryDate, decimal cost)
        {
            if (truckIds == null || !truckIds.Any())
            {
                TempData["Error"] = "لم يتم اختيار أي شاحنات";
                return RedirectToAction(nameof(Index));
            }

            var username = User.Identity?.Name ?? "Unknown";
            int successCount = 0;
            int failCount = 0;

            foreach (var truckId in truckIds)
            {
                try
                {
                    await _insuranceService.RenewInsuranceAsync(truckId, expiryDate, cost, null, username);
                    successCount++;
                }
                catch
                {
                    failCount++;
                }
            }

            TempData["Success"] = $"تم تجديد {successCount} تأمين بنجاح";
            if (failCount > 0)
            {
                TempData["Error"] = $"فشل تجديد {failCount} تأمين";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: InsuranceRenewals/Report
        public async Task<IActionResult> Report(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today.AddMonths(-6);
            var end = endDate ?? DateTime.Today;

            var allTrucks = await _truckService.GetAllTrucksAsync();
            var renewalsInPeriod = new List<InsuranceRenewalDto>();

            foreach (var truck in allTrucks)
            {
                var renewals = await _insuranceService.GetRenewalsByTruckAsync(truck.TruckId);
                foreach (var renewal in renewals.Where(r => r.RenewalDate >= start && r.RenewalDate <= end))
                {
                    renewalsInPeriod.Add(new InsuranceRenewalDto
                    {
                        InsuranceId = renewal.InsuranceId,
                        TruckId = renewal.TruckId,
                        TruckPlate = truck.PlateNumber,
                        RenewalDate = renewal.RenewalDate,
                        ExpiryDate = renewal.ExpiryDate,
                        Cost = renewal.Cost,
                        SondNumber = renewal.SondNumber,
                        CreatedBy = renewal.CreatedBy,
                        CreatedDate = renewal.CreatedDate
                    });
                }
            }

            ViewBag.StartDate = start;
            ViewBag.EndDate = end;
            ViewBag.TotalCost = renewalsInPeriod.Sum(r => r.Cost);
            ViewBag.TotalRenewals = renewalsInPeriod.Count;

            return View(renewalsInPeriod.OrderByDescending(r => r.RenewalDate));
        }
    }
}