using Khutootcompany.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ITruckRepository Trucks { get; }
        IEmployeeRepository Employees { get; }
        IInstallmentRepository Installments { get; }
        ICashTransactionRepository CashTransactions { get; }
        IWakalaRepository Wakalat { get; }
        IVisitCardRepository VisitCards { get; }
        IInsuranceRenewalRepository InsuranceRenewals { get; }
        IAuditLogRepository AuditLogs { get; }

        IRepository<TruckAssignment> TruckAssignments { get; }
        IRepository<InstallmentPayment> InstallmentPayments { get; }

        Task<int> SaveChangesAsync();
        int SaveChanges();
    }
}
