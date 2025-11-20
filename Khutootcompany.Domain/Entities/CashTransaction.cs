using Khutootcompany.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Domain.Entities
{
    public class CashTransaction : BaseEntity
    {
        public int TransactionId { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public decimal Amount { get; set; } // إيجابي = قبض، سلبي = صرف
        public TransactionType Type { get; set; }

        public int? EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }

        public int? TruckId { get; set; }
        public virtual Truck? Truck { get; set; }

        public string? Description { get; set; }
        public string? SondNumber { get; set; }
        public string? Notes { get; set; }

        // Helper Properties
        public bool IsIncome => Amount > 0;
        public bool IsExpense => Amount < 0;
    }
}
