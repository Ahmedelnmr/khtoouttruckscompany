using Khutootcompany.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Domain.Interfaces
{
    public interface IVisitCardRepository : IRepository<VisitCard>
    {
        Task<IEnumerable<VisitCard>> GetExpiredVisitCardsAsync();
        Task<IEnumerable<VisitCard>> GetExpiringSoonVisitCardsAsync(int daysThreshold = 30);
        Task<IEnumerable<VisitCard>> GetVisitCardsByEmployeeAsync(int employeeId);
    }
}
