using Khutootcompany.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.DTOs
{
    public class EmployeeDto
    {
        public int EmployeeId { get; set; }
        public string CivilId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public JobTitle JobTitle { get; set; }
        public string JobTitleDisplay { get; set; } = string.Empty;
        public Nationality? Nationality { get; set; }
        public string? NationalityDisplay { get; set; }

        public bool IsDriver { get; set; }
        public bool IsCompanyEmployee { get; set; }
        public bool IsPurchasingDriver { get; set; }

        public decimal? Salary { get; set; }

        public DateTime? ResidencyExpiryDate { get; set; }
        public DateTime? ResidencyStartDate { get; set; }

        public string? BankCardNumber { get; set; }
        public int? BankCardExpiryMonth { get; set; }
        public int? BankCardExpiryYear { get; set; }

        public string? Notes { get; set; }

        // Current Truck (if driver)
        public string? CurrentTruckPlate { get; set; }
        public int? CurrentTruckId { get; set; }

        // Color Coding Flags
        public bool IsResidencyExpired { get; set; }
        public bool IsResidencyExpiringSoon { get; set; }
        public bool IsResidencyWarning { get; set; }

        // Audit
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
