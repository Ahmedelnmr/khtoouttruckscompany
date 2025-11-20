using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.DTOs
{
    public class CreateInstallmentDto
    {
        [Required(ErrorMessage = "الشاحنة مطلوبة")]
        public int TruckId { get; set; }

        [Required(ErrorMessage = "الموظف مطلوب")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "سعر المكتب مطلوب")]
        [Range(0.001, 1000000, ErrorMessage = "السعر يجب أن يكون أكبر من صفر")]
        public decimal TotalPriceOffice { get; set; }

        [Required(ErrorMessage = "سعر الساير مطلوب")]
        [Range(0.001, 1000000, ErrorMessage = "السعر يجب أن يكون أكبر من صفر")]
        public decimal TotalPriceSayer { get; set; }

        [Required(ErrorMessage = "القسط الشهري مطلوب")]
        [Range(0.001, 10000, ErrorMessage = "القسط يجب أن يكون أكبر من صفر")]
        public decimal MonthlyQest { get; set; }

        [StringLength(100)]
        public string? FinanceSource { get; set; }

        [StringLength(50)]
        public string? SayerTransactionNumber { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;

        [Range(1, 120, ErrorMessage = "عدد الأشهر يجب أن يكون بين 1 و 120")]
        public int TotalMonths { get; set; } = 36;

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
