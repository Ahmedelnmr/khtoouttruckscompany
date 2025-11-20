using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Domain.Entities
{
    public class Wakala : BaseEntity
    {
        public int WakalaId { get; set; }

        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; } = null!;

        public int? TruckId { get; set; } // null = وكالة عامة
        public virtual Truck? Truck { get; set; }

        public DateTime IssueDate { get; set; } = DateTime.Now;
        public DateTime ExpiryDate { get; set; }
        public bool IsGeneral { get; set; } // وكالة عامة أو على شاحنة واحدة

        public bool IsPaid { get; set; } = false;
        public decimal Price { get; set; }
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
