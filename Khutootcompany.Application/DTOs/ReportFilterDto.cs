using Khutootcompany.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.DTOs
{
    public class ReportFilterDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? EmployeeId { get; set; }
        public int? TruckId { get; set; }
        public TransactionType? TransactionType { get; set; }
        public LicenseType? LicenseType { get; set; }
        public PAMStatus? PAMStatus { get; set; }
        public bool? IsExpired { get; set; }
        public bool? IsOverdue { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
    }
}
