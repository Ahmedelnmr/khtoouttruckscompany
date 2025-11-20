using Khutootcompany.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.DTOs
{
    public class TruckDto
    {
        public int TruckId { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string? AdminNumber { get; set; }
        public string Model { get; set; } = string.Empty;
        public int? Year { get; set; }
        public string? Color { get; set; }
        public LicenseType LicenseType { get; set; }
        public string LicenseTypeDisplay { get; set; } = string.Empty;

        public DateTime? InsuranceExpiryDate { get; set; }
        public int? InsuranceDay { get; set; }
        public int? InsuranceMonth { get; set; }
        public int? InsuranceYear { get; set; }

        public bool IsRegisteredInPAM { get; set; }
        public PAMStatus? PAMStatus { get; set; }
        public string? PAMStatusDisplay { get; set; }
        public DateTime? PAMRegistrationDate { get; set; }

        public string? PreviousPlateNumber { get; set; }
        public string? Notes { get; set; }

        // Current Driver
        public string? CurrentDriverName { get; set; }
        public string? CurrentDriverCivilId { get; set; }
        public string? CurrentDriverNationality { get; set; }

        // Color Coding Flags
        public bool IsInsuranceExpired { get; set; }
        public bool IsInsuranceExpiringSoon { get; set; }
        public bool IsInsuranceWarning { get; set; }

        // Audit
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
