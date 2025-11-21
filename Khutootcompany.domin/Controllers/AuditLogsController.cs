using Khutootcompany.Application.Interfaces;
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

        // GET: AuditLogs
        public async Task<IActionResult> Index(string? entityName, int? entityId, string? username,
                                                DateTime? startDate, DateTime? endDate, string? action)
        {
            IEnumerable<AuditLogDto> logs;

            // Apply filters
            if (!string.IsNullOrEmpty(entityName) && entityId.HasValue)
            {
                logs = await _auditService.GetLogsByEntityAsync(entityName, entityId.Value);
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
                logs = await _auditService.GetRecentLogsAsync(200);
            }

            // Additional filtering by action
            if (!string.IsNullOrEmpty(action))
            {
                logs = logs.Where(l => l.Action.Equals(action, StringComparison.OrdinalIgnoreCase));
            }

            // Additional filtering by entity name only
            if (!string.IsNullOrEmpty(entityName) && !entityId.HasValue)
            {
                logs = logs.Where(l => l.EntityName.Equals(entityName, StringComparison.OrdinalIgnoreCase));
            }

            ViewBag.EntityName = entityName;
            ViewBag.EntityId = entityId;
            ViewBag.Username = username;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.Action = action;

            return View(logs);
        }

        // GET: AuditLogs/ByEntity
        public async Task<IActionResult> ByEntity(string entityName, int entityId)
        {
            var logs = await _auditService.GetLogsByEntityAsync(entityName, entityId);

            ViewBag.EntityName = entityName;
            ViewBag.EntityId = entityId;
            ViewBag.FilterTitle = $"سجل التغييرات لـ {entityName} #{entityId}";

            return View("Index", logs);
        }

        // GET: AuditLogs/ByUser
        public async Task<IActionResult> ByUser(string username)
        {
            var logs = await _auditService.GetLogsByUserAsync(username);

            ViewBag.Username = username;
            ViewBag.FilterTitle = $"سجل التغييرات للمستخدم: {username}";

            return View("Index", logs);
        }

        // GET: AuditLogs/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var logs = await _auditService.GetAllLogsAsync();
            var log = logs.FirstOrDefault(l => l.AuditLogId == id);

            if (log == null)
                return NotFound();

            return View(log);
        }

        // GET: AuditLogs/Report
        public async Task<IActionResult> Report(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Today.AddDays(-30);
            var end = endDate ?? DateTime.Today;

            var logs = await _auditService.GetLogsByDateRangeAsync(start, end);

            // Statistics
            var stats = new Dictionary<string, object>
            {
                ["TotalLogs"] = logs.Count(),
                ["AddedCount"] = logs.Count(l => l.Action == "Added"),
                ["ModifiedCount"] = logs.Count(l => l.Action == "Modified"),
                ["DeletedCount"] = logs.Count(l => l.Action == "Deleted"),
                ["UniqueUsers"] = logs.Select(l => l.PerformedBy).Distinct().Count(),
                ["UniqueEntities"] = logs.Select(l => l.EntityName).Distinct().Count(),
                ["MostActiveUser"] = logs.GroupBy(l => l.PerformedBy)
                                         .OrderByDescending(g => g.Count())
                                         .FirstOrDefault()?.Key ?? "N/A",
                ["MostModifiedEntity"] = logs.GroupBy(l => l.EntityName)
                                             .OrderByDescending(g => g.Count())
                                             .FirstOrDefault()?.Key ?? "N/A"
            };

            ViewBag.StartDate = start;
            ViewBag.EndDate = end;
            ViewBag.Stats = stats;

            return View(logs);
        }

        // GET: AuditLogs/Compare
        public async Task<IActionResult> Compare(int logId1, int logId2)
        {
            var allLogs = await _auditService.GetAllLogsAsync();
            var log1 = allLogs.FirstOrDefault(l => l.AuditLogId == logId1);
            var log2 = allLogs.FirstOrDefault(l => l.AuditLogId == logId2);

            if (log1 == null || log2 == null)
                return NotFound();

            ViewBag.Log1 = log1;
            ViewBag.Log2 = log2;

            return View();
        }

        // GET: AuditLogs/EntityTypes
        public async Task<IActionResult> EntityTypes()
        {
            var logs = await _auditService.GetAllLogsAsync();
            var entityGroups = logs.GroupBy(l => l.EntityName)
                                   .Select(g => new
                                   {
                                       EntityName = g.Key,
                                       Count = g.Count(),
                                       LastModified = g.Max(l => l.PerformedAt)
                                   })
                                   .OrderByDescending(e => e.Count);

            return View(entityGroups);
        }

        // GET: AuditLogs/UserActivity
        public async Task<IActionResult> UserActivity()
        {
            var logs = await _auditService.GetAllLogsAsync();
            var userGroups = logs.GroupBy(l => l.PerformedBy)
                                 .Select(g => new
                                 {
                                     Username = g.Key,
                                     TotalActions = g.Count(),
                                     AddedCount = g.Count(l => l.Action == "Added"),
                                     ModifiedCount = g.Count(l => l.Action == "Modified"),
                                     DeletedCount = g.Count(l => l.Action == "Deleted"),
                                     LastActivity = g.Max(l => l.PerformedAt)
                                 })
                                 .OrderByDescending(u => u.TotalActions);

            return View(userGroups);
        }

        // POST: AuditLogs/Export
        [HttpPost]
        public async Task<IActionResult> Export(DateTime startDate, DateTime endDate, string format = "csv")
        {
            var logs = await _auditService.GetLogsByDateRangeAsync(startDate, endDate);

            if (format == "csv")
            {
                var csv = new System.Text.StringBuilder();
                csv.AppendLine("التاريخ,المستخدم,الكيان,رقم الكيان,الإجراء");

                foreach (var log in logs)
                {
                    csv.AppendLine($"{log.PerformedAt:yyyy-MM-dd HH:mm:ss},{log.PerformedBy},{log.EntityName},{log.EntityId},{log.Action}");
                }

                var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
                return File(bytes, "text/csv", $"audit_logs_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.csv");
            }

            return BadRequest("Unsupported format");
        }

        // GET: AuditLogs/Timeline
        public async Task<IActionResult> Timeline(DateTime? date)
        {
            var targetDate = date ?? DateTime.Today;
            var start = targetDate.Date;
            var end = start.AddDays(1).AddSeconds(-1);

            var logs = await _auditService.GetLogsByDateRangeAsync(start, end);

            ViewBag.TargetDate = targetDate;
            ViewBag.HourlyActivity = logs.GroupBy(l => l.PerformedAt.Hour)
                                         .Select(g => new { Hour = g.Key, Count = g.Count() })
                                         .OrderBy(h => h.Hour);

            return View(logs.OrderBy(l => l.PerformedAt));
        }
    }
}