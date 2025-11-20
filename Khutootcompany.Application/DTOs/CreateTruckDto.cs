using Khutootcompany.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.DTOs
{
    public class CreateTruckDto
    {
        [Required(ErrorMessage = "رقم اللوحة مطلوب")]
        [StringLength(20, ErrorMessage = "رقم اللوحة يجب ألا يتجاوز 20 حرف")]
        public string PlateNumber { get; set; } = string.Empty;

        [StringLength(10)]
        public string? AdminNumber { get; set; }

        [Required(ErrorMessage = "الموديل مطلوب")]
        [StringLength(100)]
        public string Model { get; set; } = string.Empty;

        [Range(1990, 2030, ErrorMessage = "السنة يجب أن تكون بين 1990 و 2030")]
        public int? Year { get; set; }

        [StringLength(50)]
        public string? Color { get; set; }

        [Required(ErrorMessage = "نوع الترخيص مطلوب")]
        public LicenseType LicenseType { get; set; }

        public DateTime? InsuranceExpiryDate { get; set; }
        public int? InsuranceDay { get; set; }
        public int? InsuranceMonth { get; set; }
        public int? InsuranceYear { get; set; }

        public bool IsRegisteredInPAM { get; set; }
        public PAMStatus? PAMStatus { get; set; }
        public DateTime? PAMRegistrationDate { get; set; }

        [StringLength(20)]
        public string? PreviousPlateNumber { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
