using System;

namespace Khutootcompany.Application.DTOs
{
    public class WakalaDto
    {
        public int WakalaId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;

        public int? TruckId { get; set; }
        public string? TruckPlate { get; set; }

        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsGeneral { get; set; }

        public bool IsPaid { get; set; }
        public decimal Price { get; set; }
        public string? SondNumber { get; set; }
        public string? Notes { get; set; }

        // Color Coding Flags
        public bool IsExpired { get; set; }
        public bool IsExpiringSoon { get; set; }
        public bool IsWarning { get; set; }

        // ⭐ إضافات مفيدة للعرض
        public int DaysRemaining => IsExpired ? 0 : (ExpiryDate - DateTime.Today).Days;
        public int DaysUntilExpiry => (ExpiryDate - DateTime.Today).Days;
        public bool IsValid => !IsExpired;
        public string WakalaType => IsGeneral ? "وكالة عامة" : "وكالة على شاحنة";
        public string StatusText => IsExpired ? "منتهي" :
                                    IsExpiringSoon ? "ينتهي قريباً" :
                                    IsWarning ? "تحذير" : "صالح";
        public string StatusColor => IsExpired ? "danger" :
                                     IsExpiringSoon ? "warning" :
                                     IsWarning ? "info" : "success";

        // ⭐ معلومات مفيدة إضافية
        public int DurationInDays => (ExpiryDate - IssueDate).Days;
        public int DurationInMonths => DurationInDays / 30;

        // Audit
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}