using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.DTOs
{
    public class EmployeeDetailsDto : EmployeeDto
    {
        // Current Truck Assignment (if driver)
        public TruckAssignmentDto? CurrentAssignment { get; set; }

        // Assignment History
        public List<TruckAssignmentDto> AssignmentHistory { get; set; } = new();

        // Installments (if purchasing driver)
        public List<InstallmentDto> Installments { get; set; } = new();

        // Wakalat
        public List<WakalaDto> Wakalat { get; set; } = new();

        // Visit Cards
        public List<VisitCardDto> VisitCards { get; set; } = new();

        // Cash Transactions related to this employee
        public List<CashTransactionDto> CashTransactions { get; set; } = new();
    }
}
