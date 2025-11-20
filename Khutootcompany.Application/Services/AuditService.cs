using Khutootcompany.Application.Interfaces;
using Khutootcompany.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.Services
{

    public class AuditService : IAuditService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuditService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<AuditLogDto>> GetAllLogsAsync()
        {
            var logs = await _unitOfWork.AuditLogs.GetAllAsync();
            return logs.Select(l => new AuditLogDto
            {
                AuditLogId = l.AuditLogId,
                EntityName = l.EntityName,
                EntityId = l.EntityId,
                Action = l.Action,
                OldValues = l.OldValues,
                NewValues = l.NewValues,
                PerformedBy = l.PerformedBy,
                PerformedAt = l.PerformedAt
            });
        }

        public async Task<IEnumerable<AuditLogDto>> GetLogsByEntityAsync(string entityName, int entityId)
        {
            var logs = await _unitOfWork.AuditLogs.GetLogsByEntityAsync(entityName, entityId);
            return logs.Select(l => new AuditLogDto
            {
                AuditLogId = l.AuditLogId,
                EntityName = l.EntityName,
                EntityId = l.EntityId,
                Action = l.Action,
                OldValues = l.OldValues,
                NewValues = l.NewValues,
                PerformedBy = l.PerformedBy,
                PerformedAt = l.PerformedAt
            });
        }

        public async Task<IEnumerable<AuditLogDto>> GetLogsByUserAsync(string username)
        {
            var logs = await _unitOfWork.AuditLogs.GetLogsByUserAsync(username);
            return logs.Select(l => new AuditLogDto
            {
                AuditLogId = l.AuditLogId,
                EntityName = l.EntityName,
                EntityId = l.EntityId,
                Action = l.Action,
                OldValues = l.OldValues,
                NewValues = l.NewValues,
                PerformedBy = l.PerformedBy,
                PerformedAt = l.PerformedAt
            });
        }

        public async Task<IEnumerable<AuditLogDto>> GetLogsByDateRangeAsync(DateTime start, DateTime end)
        {
            var logs = await _unitOfWork.AuditLogs.GetLogsByDateRangeAsync(start, end);
            return logs.Select(l => new AuditLogDto
            {
                AuditLogId = l.AuditLogId,
                EntityName = l.EntityName,
                EntityId = l.EntityId,
                Action = l.Action,
                OldValues = l.OldValues,
                NewValues = l.NewValues,
                PerformedBy = l.PerformedBy,
                PerformedAt = l.PerformedAt
            });
        }

        public async Task<IEnumerable<AuditLogDto>> GetRecentLogsAsync(int count = 100)
        {
            var logs = await _unitOfWork.AuditLogs.GetRecentLogsAsync(count);
            return logs.Select(l => new AuditLogDto
            {
                AuditLogId = l.AuditLogId,
                EntityName = l.EntityName,
                EntityId = l.EntityId,
                Action = l.Action,
                OldValues = l.OldValues,
                NewValues = l.NewValues,
                PerformedBy = l.PerformedBy,
                PerformedAt = l.PerformedAt
            });
        }
    }
}
