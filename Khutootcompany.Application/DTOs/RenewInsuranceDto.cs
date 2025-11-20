using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.DTOs
{
    public class RenewInsuranceDto
    {
        [Required]
        public int TruckId { get; set; }

        [Required(ErrorMessage = "تاريخ الانتهاء مطلوب")]
        public DateTime ExpiryDate { get; set; }

        [Required(ErrorMessage = "التكلفة مطلوبة")]
        [Range(0.001, 10000, ErrorMessage = "التكلفة يجب أن تكون أكبر من صفر")]
        public decimal Cost { get; set; }

        [StringLength(50)]
        public string? SondNumber { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
