using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.DTOs
{
    public class RecordPaymentDto
    {
        [Required]
        public int InstallmentId { get; set; }

        [Required(ErrorMessage = "المبلغ مطلوب")]
        [Range(0.001, 100000, ErrorMessage = "المبلغ يجب أن يكون أكبر من صفر")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "نوع الدفعة مطلوب")]
        [StringLength(50)]
        public string PaymentType { get; set; } = "قسط";

        [StringLength(50)]
        public string? SondNumber { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
