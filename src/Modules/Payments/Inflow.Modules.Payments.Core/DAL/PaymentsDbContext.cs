using Inflow.Modules.Payments.Core.Deposits.Domain.Entities;
using Inflow.Modules.Payments.Core.Withdrawals.Domain.Entities;
using Inflow.Modules.Payments.Infrastructure.Entities;
using Inflow.Modules.Payments.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Inflow.Modules.Payments.Core.DAL;

internal class PaymentsDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Deposit> Deposits { get; set; }
    public DbSet<DepositAccount> DepositAccounts { get; set; }
    public DbSet<Withdrawal> Withdrawals { get; set; }
    public DbSet<WithdrawalAccount> WithdrawalAccounts { get; set; }

    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("payments");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}