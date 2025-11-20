using Khutootcompany.Domain.Entities;
using Khutootcompany.Domain.Interfaces;
using Khutootcompany.Infrastructure.Data;
using Khutootcompany.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khutootcompany.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private ITruckRepository? _trucks;
        private IEmployeeRepository? _employees;
        private IInstallmentRepository? _installments;
        private ICashTransactionRepository? _cashTransactions;
        private IWakalaRepository? _wakalat;
        private IVisitCardRepository? _visitCards;
        private IInsuranceRenewalRepository? _insuranceRenewals;
        private IAuditLogRepository? _auditLogs;
        private IRepository<TruckAssignment>? _truckAssignments;
        private IRepository<InstallmentPayment>? _installmentPayments;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public ITruckRepository Trucks =>
            _trucks ??= (ITruckRepository)new TruckRepository(_context);

        public IEmployeeRepository Employees =>
            _employees ??= new EmployeeRepository(_context);

        public IInstallmentRepository Installments =>
            _installments ??= new InstallmentRepository(_context);

        public ICashTransactionRepository CashTransactions =>
            _cashTransactions ??= new CashTransactionRepository(_context);

        public IWakalaRepository Wakalat =>
            _wakalat ??= new WakalaRepository(_context);

        public IVisitCardRepository VisitCards =>
            _visitCards ??= new VisitCardRepository(_context);

        public IInsuranceRenewalRepository InsuranceRenewals =>
            _insuranceRenewals ??= new InsuranceRenewalRepository(_context);

        public IAuditLogRepository AuditLogs =>
            _auditLogs ??= new AuditLogRepository(_context);

        public IRepository<TruckAssignment> TruckAssignments =>
            _truckAssignments ??= new GenericRepository<TruckAssignment>(_context);

        public IRepository<InstallmentPayment> InstallmentPayments =>
            _installmentPayments ??= new GenericRepository<InstallmentPayment>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
