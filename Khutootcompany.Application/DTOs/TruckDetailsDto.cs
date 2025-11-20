using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.DTOs
{
    public class TruckDetailsDto : TruckDto
    {
        // Current Assignment
        public TruckAssignmentDto? CurrentAssignment { get; set; }

        // Assignment History
        public List<TruckAssignmentDto> AssignmentHistory { get; set; } = new();

        // Installment
        public InstallmentDto? Installment { get; set; }

        // Wakalat
        public List<WakalaDto> Wakalat { get; set; } = new();

        // Visit Cards
        public List<VisitCardDto> VisitCards { get; set; } = new();

        // Insurance Renewals
        public List<InsuranceRenewalDto> InsuranceRenewals { get; set; } = new();

        // Cash Transactions related to this truck
        public List<CashTransactionDto> CashTransactions { get; set; } = new();
    }
}
