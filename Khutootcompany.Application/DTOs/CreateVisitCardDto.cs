using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.DTOs
{
    public class CreateVisitCardDto
    {
        [Required(ErrorMessage = "الموظف مطلوب")]
        public int EmployeeId { get; set; }

        public int? TruckId { get; set; }

        [Required(ErrorMessage = "تاريخ الإصدار مطلوب")]
        public DateTime IssueDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "تاريخ الانتهاء مطلوب")]
        public DateTime ExpiryDate { get; set; }

        // Always 15 KD
        public decimal Price { get; set; } = 15m;

        public bool IsPaid { get; set; }

        [StringLength(100)]
        public string? IntermediaryName { get; set; }

        [Phone]
        [StringLength(20)]
        public string? IntermediaryPhone { get; set; }

        [StringLength(50)]
        public string? SondNumber { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
