using Khutootcompany.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.DTOs
{
    public class CreateEmployeeDto
    {
        [Required(ErrorMessage = "الرقم المدني مطلوب")]
        [StringLength(20)]
        public string CivilId { get; set; } = string.Empty;

        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "الوظيفة مطلوبة")]
        public JobTitle JobTitle { get; set; }

        public Nationality? Nationality { get; set; }

        public bool IsDriver { get; set; }
        public bool IsCompanyEmployee { get; set; }
        public bool IsPurchasingDriver { get; set; }

        [Range(0, 10000, ErrorMessage = "الراتب يجب أن يكون بين 0 و 10000")]
        public decimal? Salary { get; set; }

        public DateTime? ResidencyExpiryDate { get; set; }
        public DateTime? ResidencyStartDate { get; set; }

        [StringLength(30)]
        public string? BankCardNumber { get; set; }

        [Range(1, 12)]
        public int? BankCardExpiryMonth { get; set; }

        [Range(2024, 2050)]
        public int? BankCardExpiryYear { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
