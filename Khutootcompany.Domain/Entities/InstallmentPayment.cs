using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Domain.Entities
{
    public class InstallmentPayment : BaseEntity
    {
        public int PaymentId { get; set; }

        public int InstallmentId { get; set; }
        public virtual Installment Installment { get; set; } = null!;

        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public decimal Amount { get; set; }
        public string PaymentType { get; set; } = "قسط"; // قسط، مقدم، إلخ
        public string? SondNumber { get; set; }
        public string? Notes { get; set; }
    }
}
