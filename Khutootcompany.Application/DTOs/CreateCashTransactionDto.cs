using Khutootcompany.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.DTOs
{
    public class CreateCashTransactionDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "تاريخ الحركة مطلوب")]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "المبلغ مطلوب")]
        [Range(-1000000, 1000000, ErrorMessage = "المبلغ يجب أن يكون بين -1000000 و 1000000")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "نوع الحركة مطلوب")]
        public TransactionType Type { get; set; }

        public int? EmployeeId { get; set; }
        public int? TruckId { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? SondNumber { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
