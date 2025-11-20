using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.DTOs
{
    public class VisitCardDto
    {
        public int VisitCardId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;

        public int? TruckId { get; set; }
        public string? TruckPlate { get; set; }

        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        public decimal Price { get; set; }
        public bool IsPaid { get; set; }

        public string? IntermediaryName { get; set; }
        public string? IntermediaryPhone { get; set; }

        public string? SondNumber { get; set; }
        public string? Notes { get; set; }

        // Color Coding Flags
        public bool IsExpired { get; set; }
        public bool IsExpiringSoon { get; set; }
        public bool IsWarning { get; set; }

        // Audit
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
