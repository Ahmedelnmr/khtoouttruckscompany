using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.DTOs
{
    public class TruckAssignmentDto
    {
        public int AssignmentId { get; set; }

        public int TruckId { get; set; }
        public string TruckPlate { get; set; } = string.Empty;
        public string TruckModel { get; set; } = string.Empty;

        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCivilId { get; set; } = string.Empty;

        public DateTime AssignmentDate { get; set; }
        public bool IsCurrent { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Notes { get; set; }

        // Audit
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
