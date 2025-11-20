using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.DTOs
{
    public class InstallmentDto
    {
        public int InstallmentId { get; set; }
        public int TruckId { get; set; }
        public string TruckPlate { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;

        public decimal TotalPriceOffice { get; set; }
        public decimal TotalPriceSayer { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal MonthlyQest { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal ProgressPercentage { get; set; }

        public string? FinanceSource { get; set; }
        public string? SayerTransactionNumber { get; set; }

        public DateTime StartDate { get; set; }
        public int TotalMonths { get; set; }

        public bool IsCompleted { get; set; }
        public bool IsOverdue { get; set; }
        public string? Notes { get; set; }

        public List<InstallmentPaymentDto> Payments { get; set; } = new();

        // Audit
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
