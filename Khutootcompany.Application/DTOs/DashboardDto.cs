using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Application.DTOs
{
    public class DashboardDto
    {
        public int TotalTrucks { get; set; }
        public int ExpiredResidencies { get; set; }
        public int ExpiredInsurance { get; set; }
        public int OverdueInstallments { get; set; }
        public decimal CurrentCashBalance { get; set; }
        public int ExpiredWakalat { get; set; }
        public int ActiveDrivers { get; set; }
        public int AdminStaff { get; set; }

        public List<CashTransactionDto> LatestTransactions { get; set; } = new();
    }
}
