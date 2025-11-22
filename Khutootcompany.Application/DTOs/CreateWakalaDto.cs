using System;
using System.ComponentModel.DataAnnotations;

namespace Khutootcompany.Application.DTOs
{
    public class CreateWakalaDto
    {
        // ⭐ إضافة ID للتعديل
        public int WakalaId { get; set; }

        [Required(ErrorMessage = "الموظف مطلوب")]
        public int EmployeeId { get; set; }

        // ⭐ يكون null في حالة الوكالة العامة
        public int? TruckId { get; set; }

        [Required(ErrorMessage = "تاريخ الإصدار مطلوب")]
        public DateTime IssueDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "تاريخ الانتهاء مطلوب")]
        public DateTime ExpiryDate { get; set; }

        public bool IsGeneral { get; set; }

        public bool IsPaid { get; set; }

        [Required(ErrorMessage = "السعر مطلوب")]
        [Range(0.001, 1000, ErrorMessage = "السعر يجب أن يكون بين 0.001 و 1000")]
        public decimal Price { get; set; }

        [StringLength(50)]
        public string? SondNumber { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // ⭐ Validation: إذا كانت الوكالة على شاحنة، يجب تحديد الشاحنة
        public bool IsValid()
        {
            // إذا الوكالة مش عامة، لازم يكون في TruckId
            if (!IsGeneral && !TruckId.HasValue)
                return false;

            // تاريخ الانتهاء لازم يكون بعد تاريخ الإصدار
            if (ExpiryDate <= IssueDate)
                return false;

            return true;
        }
    }
}