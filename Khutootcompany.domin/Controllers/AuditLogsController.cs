using Khutootcompany.Application.Interfaces;
using Khutootcompany.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Khutootcompany.presention.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AuditLogsController : Controller
    {
        private readonly IAuditService _auditService;

        public AuditLogsController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        public async Task<IActionResult> Index(string? entityName, string? username, DateTime? startDate, DateTime? endDate)
        {
            IEnumerable<AuditLogDto> logs;

            if (!string.IsNullOrEmpty(entityName))
            {
                logs = (await _auditService.GetAllLogsAsync())
                    .Where(l => l.EntityName.Equals(entityName, StringComparison.OrdinalIgnoreCase));
            }
            else if (!string.IsNullOrEmpty(username))
            {
                logs = await _auditService.GetLogsByUserAsync(username);
            }
            else if (startDate.HasValue && endDate.HasValue)
            {
                logs = await _auditService.GetLogsByDateRangeAsync(startDate.Value, endDate.Value);
            }
            else
            {
                logs = await _auditService.GetRecentLogsAsync(100);
            }

            return View(logs);
        }
    }
}
