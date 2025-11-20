using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Domain.Entities
{
    public class VisitCard : BaseEntity
    {
        public int VisitCardId { get; set; }

        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; } = null!;

        public int? TruckId { get; set; }
        public virtual Truck? Truck { get; set; }

        public DateTime IssueDate { get; set; } = DateTime.Now;
        public DateTime ExpiryDate { get; set; } // 3 شهور من تاريخ الإصدار

        public decimal Price { get; set; } = 15m; // 15 دينار
        public bool IsPaid { get; set; } = false;

        public string? IntermediaryName { get; set; }  // اسم الوسيط
        public string? IntermediaryPhone { get; set; } // رقم الوسيط

        public string? SondNumber { get; set; }
        public string? Notes { get; set; }

        // Helper Methods for Color Coding
        public bool IsExpiringSoon()
        {
            var daysUntilExpiry = (ExpiryDate - DateTime.Today).Days;
            return daysUntilExpiry <= 30 && daysUntilExpiry >= 0;
        }

        public bool IsWarning()
        {
            var daysUntilExpiry = (ExpiryDate - DateTime.Today).Days;
            return daysUntilExpiry <= 60 && daysUntilExpiry > 30;
        }

        public bool IsExpired()
        {
            return ExpiryDate < DateTime.Today;
        }
    }
}
