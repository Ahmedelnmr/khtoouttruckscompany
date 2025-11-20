using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.DTOs
{
    public class StatisticsDto
    {
        // Trucks
        public int TotalTrucks { get; set; }
        public int TrucksWithExpiredInsurance { get; set; }
        public int TrucksWithExpiringSoonInsurance { get; set; }
        public int TrucksNotAcceptedInPAM { get; set; }

        // Employees
        public int TotalEmployees { get; set; }
        public int TotalDrivers { get; set; }
        public int EmployeesWithExpiredResidency { get; set; }
        public int EmployeesWithExpiringSoonResidency { get; set; }

        // Financial
        public decimal CurrentCashBalance { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public int ActiveInstallments { get; set; }
        public int OverdueInstallments { get; set; }
        public decimal TotalInstallmentDebt { get; set; }

        // Documents
        public int ExpiredWakalat { get; set; }
        public int ExpiringSoonWakalat { get; set; }
        public int ExpiredVisitCards { get; set; }
        public int ExpiringSoonVisitCards { get; set; }
    }
}
