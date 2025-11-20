using Khutootcompany.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.Interfaces
{
    public interface IVisitCardService
    {
        Task<IEnumerable<VisitCardDto>> GetAllVisitCardsAsync();
        Task<VisitCardDto?> GetVisitCardByIdAsync(int id);
        Task<IEnumerable<VisitCardDto>> GetExpiredVisitCardsAsync();
        Task<IEnumerable<VisitCardDto>> GetExpiringSoonVisitCardsAsync(int days = 30);
        Task<IEnumerable<VisitCardDto>> GetVisitCardsByEmployeeAsync(int employeeId);
        Task<VisitCardDto> CreateVisitCardAsync(CreateVisitCardDto dto, string username);
        Task<VisitCardDto> UpdateVisitCardAsync(VisitCardDto dto, string username);
        Task DeleteVisitCardAsync(int id, string username);
    }
}
