using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Domain.Entities
{
    public class Installment : BaseEntity
    {
        public int InstallmentId { get; set; }

        public int TruckId { get; set; }
        public virtual Truck Truck { get; set; } = null!;

        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; } = null!;

        // Prices
        public decimal TotalPriceOffice { get; set; } // سعر المكتب (أعلى)
        public decimal TotalPriceSayer { get; set; }  // سعر الساير (أقل)
        public decimal MonthlyQest { get; set; }       // القسط الشهري
        public decimal PaidAmount { get; set; }        // المدفوع
        public decimal RemainingAmount { get; set; }   // المتبقي

        public string? FinanceSource { get; set; }     // الساير، أنوار السالمي، السلام، مباشر
        public string? SayerTransactionNumber { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;
        public int TotalMonths { get; set; } = 36; // Default 36 months

        public bool IsCompleted { get; set; } = false;
        public string? Notes { get; set; }

        // Navigation Properties
        public virtual ICollection<InstallmentPayment> Payments { get; set; } = new List<InstallmentPayment>();

        // Helper Property
        public decimal ProfitMargin => TotalPriceOffice - TotalPriceSayer;

        public bool IsOverdue()
        {
            if (IsCompleted) return false;
            // Calculate expected paid amount based on months passed
            var monthsPassed = (int)((DateTime.Now - StartDate).Days / 30.0);
            var expectedPaid = monthsPassed * MonthlyQest;
            return PaidAmount < expectedPaid;
        }
    }
}
