using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.DTOs
{
    public class CreateWakalaDto
    {
        [Required(ErrorMessage = "الموظف مطلوب")]
        public int EmployeeId { get; set; }

        // Null = وكالة عامة
        public int? TruckId { get; set; }

        [Required(ErrorMessage = "تاريخ الإصدار مطلوب")]
        public DateTime IssueDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "تاريخ الانتهاء مطلوب")]
        public DateTime ExpiryDate { get; set; }

        public bool IsGeneral { get; set; }

        public bool IsPaid { get; set; }

        [Required(ErrorMessage = "السعر مطلوب")]
        [Range(0.001, 1000, ErrorMessage = "السعر يجب أن يكون أكبر من صفر")]
        public decimal Price { get; set; }

        [StringLength(50)]
        public string? SondNumber { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
