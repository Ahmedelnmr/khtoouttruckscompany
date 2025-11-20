// ============================================
// KhutootTrucks.Infrastructure/Data/ApplicationDbContext.cs
// ============================================
using Khutootcompany.Domain.Entities;

//using KhutootTrucks.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Khutootcompany.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IHttpContextAccessor? httpContextAccessor = null)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // DbSets
        public DbSet<Truck> Trucks { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<TruckAssignment> TruckAssignments { get; set; } = null!;
        public DbSet<Installment> Installments { get; set; } = null!;
        public DbSet<InstallmentPayment> InstallmentPayments { get; set; } = null!;
        public DbSet<Wakala> Wakalat { get; set; } = null!;
        public DbSet<VisitCard> VisitCards { get; set; } = null!;
        public DbSet<CashTransaction> CashTransactions { get; set; } = null!;
        public DbSet<InsuranceRenewal> InsuranceRenewals { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Truck Configuration
            modelBuilder.Entity<Truck>(entity =>
            {
                entity.HasKey(e => e.TruckId);
                entity.HasIndex(e => e.PlateNumber).IsUnique();
                entity.Property(e => e.PlateNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Model).IsRequired().HasMaxLength(100);
                entity.Property(e => e.AdminNumber).HasMaxLength(10);
                entity.Property(e => e.Color).HasMaxLength(50);

                // Global Query Filter for Soft Delete
                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Employee Configuration
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmployeeId);
                entity.HasIndex(e => e.CivilId).IsUnique();
                entity.Property(e => e.CivilId).IsRequired().HasMaxLength(20);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.BankCardNumber).HasMaxLength(30);
                entity.Property(e => e.Salary).HasPrecision(18, 3);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // TruckAssignment Configuration
            modelBuilder.Entity<TruckAssignment>(entity =>
            {
                entity.HasKey(e => e.AssignmentId);

                entity.HasOne(e => e.Truck)
                    .WithMany(t => t.Assignments)
                    .HasForeignKey(e => e.TruckId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Employee)
                    .WithMany(emp => emp.TruckAssignments)
                    .HasForeignKey(e => e.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Installment Configuration
            modelBuilder.Entity<Installment>(entity =>
            {
                entity.HasKey(e => e.InstallmentId);

                entity.Property(e => e.TotalPriceOffice).HasPrecision(18, 3);
                entity.Property(e => e.TotalPriceSayer).HasPrecision(18, 3);
                entity.Property(e => e.MonthlyQest).HasPrecision(18, 3);
                entity.Property(e => e.PaidAmount).HasPrecision(18, 3);
                entity.Property(e => e.RemainingAmount).HasPrecision(18, 3);

                entity.HasOne(e => e.Truck)
                    .WithMany(t => t.Installments)
                    .HasForeignKey(e => e.TruckId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Employee)
                    .WithMany(emp => emp.Installments)
                    .HasForeignKey(e => e.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // InstallmentPayment Configuration
            modelBuilder.Entity<InstallmentPayment>(entity =>
            {
                entity.HasKey(e => e.PaymentId);
                entity.Property(e => e.Amount).HasPrecision(18, 3);

                entity.HasOne(e => e.Installment)
                    .WithMany(i => i.Payments)
                    .HasForeignKey(e => e.InstallmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // Wakala Configuration
            modelBuilder.Entity<Wakala>(entity =>
            {
                entity.HasKey(e => e.WakalaId);
                entity.Property(e => e.Price).HasPrecision(18, 3);

                entity.HasOne(e => e.Employee)
                    .WithMany(emp => emp.Wakalat)
                    .HasForeignKey(e => e.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Truck)
                    .WithMany(t => t.Wakalat)
                    .HasForeignKey(e => e.TruckId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // VisitCard Configuration
            modelBuilder.Entity<VisitCard>(entity =>
            {
                entity.HasKey(e => e.VisitCardId);
                entity.Property(e => e.Price).HasPrecision(18, 3).HasDefaultValue(15m);

                entity.HasOne(e => e.Employee)
                    .WithMany(emp => emp.VisitCards)
                    .HasForeignKey(e => e.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Truck)
                    .WithMany(t => t.VisitCards)
                    .HasForeignKey(e => e.TruckId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // CashTransaction Configuration
            modelBuilder.Entity<CashTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId);
                entity.Property(e => e.Amount).HasPrecision(18, 3);

                entity.HasOne(e => e.Employee)
                    .WithMany()
                    .HasForeignKey(e => e.EmployeeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Truck)
                    .WithMany()
                    .HasForeignKey(e => e.TruckId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // InsuranceRenewal Configuration
            modelBuilder.Entity<InsuranceRenewal>(entity =>
            {
                entity.HasKey(e => e.InsuranceId);
                entity.Property(e => e.Cost).HasPrecision(18, 3);

                entity.HasOne(e => e.Truck)
                    .WithMany(t => t.InsuranceRenewals)
                    .HasForeignKey(e => e.TruckId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasQueryFilter(e => !e.IsDeleted);
            });

            // AuditLog Configuration
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.AuditLogId);
                entity.Property(e => e.EntityName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PerformedBy).IsRequired().HasMaxLength(256);
                entity.HasIndex(e => new { e.EntityName, e.EntityId });
                entity.HasIndex(e => e.PerformedAt);
            });
        }

        // ============================================
        // CRITICAL: Audit Override for SaveChanges
        // ============================================
        public override int SaveChanges()
        {
            ApplyAuditInfo();
            LogChanges();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var username = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";

            var auditEntries = ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
                .Select(entry => new AuditLog { /* نفس الكود */ })
                .ToList();

            ApplyAuditInfo();

            var result = await base.SaveChangesAsync(cancellationToken);

            if (auditEntries.Any())
            {
                AuditLogs.AddRange(auditEntries);
                await base.SaveChangesAsync(cancellationToken);
            }

            return result;
        }
        private void ApplyAuditInfo()
        {
            var username = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = username;
                        entry.Entity.CreatedDate = DateTime.Now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedBy = username;
                        entry.Entity.UpdatedDate = DateTime.Now;
                        break;

                    case EntityState.Deleted:
                        // Soft Delete
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedBy = username;
                        entry.Entity.DeletedDate = DateTime.Now;
                        break;
                }
            }
        }

        private void LogChanges()
        {
            var username = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "System";
            var entries = ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Added ||
                           e.State == EntityState.Modified ||
                           e.State == EntityState.Deleted);

            foreach (var entry in entries)
            {
                var auditLog = new AuditLog
                {
                    EntityName = entry.Entity.GetType().Name,
                    EntityId = GetEntityId(entry.Entity),
                    Action = entry.State.ToString(),
                    OldValues = entry.State == EntityState.Modified || entry.State == EntityState.Deleted
                        ? JsonSerializer.Serialize(entry.OriginalValues.Properties.ToDictionary(p => p.Name, p => entry.OriginalValues[p]))
                        : null,
                    NewValues = entry.State == EntityState.Added || entry.State == EntityState.Modified
                        ? JsonSerializer.Serialize(entry.CurrentValues.Properties.ToDictionary(p => p.Name, p => entry.CurrentValues[p]))
                        : null,
                    PerformedBy = username,
                    PerformedAt = DateTime.Now
                };

                AuditLogs.Add(auditLog);
            }
        }

        private int GetEntityId(BaseEntity entity)
        {
            return entity switch
            {
                Truck truck => truck.TruckId,
                Employee emp => emp.EmployeeId,
                TruckAssignment assignment => assignment.AssignmentId,
                Installment installment => installment.InstallmentId,
                InstallmentPayment payment => payment.PaymentId,
                Wakala wakala => wakala.WakalaId,
                VisitCard card => card.VisitCardId,
                CashTransaction trans => trans.TransactionId,
                InsuranceRenewal renewal => renewal.InsuranceId,
                _ => 0
            };
        }
    }

    // ============================================
    // ApplicationUser for Identity
    // ============================================
    public class ApplicationUser : Microsoft.AspNetCore.Identity.IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
    }
}