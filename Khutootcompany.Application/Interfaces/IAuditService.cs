using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.Interfaces
{
    public interface IAuditService
    {
        Task<IEnumerable<AuditLogDto>> GetAllLogsAsync();
        Task<IEnumerable<AuditLogDto>> GetLogsByEntityAsync(string entityName, int entityId);
        Task<IEnumerable<AuditLogDto>> GetLogsByUserAsync(string username);
        Task<IEnumerable<AuditLogDto>> GetLogsByDateRangeAsync(DateTime start, DateTime end);
        Task<IEnumerable<AuditLogDto>> GetRecentLogsAsync(int count = 100);
    }
}
