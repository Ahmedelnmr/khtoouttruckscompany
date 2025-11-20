using Khutootcompany.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Domain.Entities
{
    public class Employee : BaseEntity
    {
        public int EmployeeId { get; set; }
        public string CivilId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public JobTitle JobTitle { get; set; }
        public Nationality? Nationality { get; set; }
        public bool IsDriver { get; set; }

        // Employee Type
        public bool IsCompanyEmployee { get; set; } // موظف رسمي تابع للشركة
        public bool IsPurchasingDriver { get; set; } // سائق مشتري العربية بالتقسيط

        public decimal? Salary { get; set; }

        // Residency
        public DateTime? ResidencyExpiryDate { get; set; }
        public DateTime? ResidencyStartDate { get; set; }

        // Bank Card
        public string? BankCardNumber { get; set; }
        public int? BankCardExpiryMonth { get; set; }
        public int? BankCardExpiryYear { get; set; }

        public string? Notes { get; set; }

        // Navigation Properties
        public virtual ICollection<TruckAssignment> TruckAssignments { get; set; } = new List<TruckAssignment>();
        public virtual ICollection<Installment> Installments { get; set; } = new List<Installment>();
        public virtual ICollection<Wakala> Wakalat { get; set; } = new List<Wakala>();
        public virtual ICollection<VisitCard> VisitCards { get; set; } = new List<VisitCard>();

        // Helper Methods for Color Coding
        public bool IsResidencyExpiringSoon()
        {
            if (!ResidencyExpiryDate.HasValue) return false;
            var daysUntilExpiry = (ResidencyExpiryDate.Value - DateTime.Today).Days;
            return daysUntilExpiry <= 30 && daysUntilExpiry >= 0;
        }

        public bool IsResidencyWarning()
        {
            if (!ResidencyExpiryDate.HasValue) return false;
            var daysUntilExpiry = (ResidencyExpiryDate.Value - DateTime.Today).Days;
            return daysUntilExpiry <= 60 && daysUntilExpiry > 30;
        }

        public bool IsResidencyExpired()
        {
            if (!ResidencyExpiryDate.HasValue) return false;
            return ResidencyExpiryDate.Value < DateTime.Today;
        }
    }
}
