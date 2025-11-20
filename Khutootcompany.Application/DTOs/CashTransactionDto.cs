using Khutootcompany.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.DTOs
{

    public class CashTransactionDto
    {
        public int TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public string TypeDisplay { get; set; } = string.Empty;

        public int? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }

        public int? TruckId { get; set; }
        public string? TruckPlate { get; set; }

        public string? Description { get; set; }
        public string? SondNumber { get; set; }
        public string? Notes { get; set; }

        public bool IsIncome { get; set; }
        public bool IsExpense { get; set; }

        // Audit
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
