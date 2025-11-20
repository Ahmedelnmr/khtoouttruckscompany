using Khutootcompany.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Domain.Entities
{

    public class Truck : BaseEntity
    {
        public int TruckId { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string? AdminNumber { get; set; } // 91 or 95
        public string Model { get; set; } = string.Empty;
        public int? Year { get; set; }
        public string? Color { get; set; }
        public LicenseType LicenseType { get; set; }

        // Insurance
        public DateTime? InsuranceExpiryDate { get; set; }
        public int? InsuranceDay { get; set; }
        public int? InsuranceMonth { get; set; }
        public int? InsuranceYear { get; set; }

        // PAM Registration
        public bool IsRegisteredInPAM { get; set; }
        public PAMStatus? PAMStatus { get; set; }
        public DateTime? PAMRegistrationDate { get; set; }

        public string? PreviousPlateNumber { get; set; }
        public string? Notes { get; set; }

        // Navigation Properties
        public virtual ICollection<TruckAssignment> Assignments { get; set; } = new List<TruckAssignment>();
        public virtual ICollection<Installment> Installments { get; set; } = new List<Installment>();
        public virtual ICollection<Wakala> Wakalat { get; set; } = new List<Wakala>();
        public virtual ICollection<VisitCard> VisitCards { get; set; } = new List<VisitCard>();
        public virtual ICollection<InsuranceRenewal> InsuranceRenewals { get; set; } = new List<InsuranceRenewal>();

        // Helper Methods for Color Coding
        public bool IsInsuranceExpiringSoon()
        {
            if (!InsuranceExpiryDate.HasValue) return false;
            var daysUntilExpiry = (InsuranceExpiryDate.Value - DateTime.Today).Days;
            return daysUntilExpiry <= 30 && daysUntilExpiry >= 0;
        }

        public bool IsInsuranceWarning()
        {
            if (!InsuranceExpiryDate.HasValue) return false;
            var daysUntilExpiry = (InsuranceExpiryDate.Value - DateTime.Today).Days;
            return daysUntilExpiry <= 60 && daysUntilExpiry > 30;
        }

        public bool IsInsuranceExpired()
        {
            if (!InsuranceExpiryDate.HasValue) return false;
            return InsuranceExpiryDate.Value < DateTime.Today;
        }
    }
}
