using System;
using System.Threading.Tasks;
using Inflow.Modules.Payments.Core.Withdrawals.Domain.Entities;
using Inflow.Modules.Payments.Core.Withdrawals.Domain.Repositories;
using Inflow.Shared.Abstractions.Kernel.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Inflow.Modules.Payments.Core.DAL.Repositories;

internal class WithdrawalAccountRepository : IWithdrawalAccountRepository
{
    private readonly PaymentsDbContext _ctx;

    public WithdrawalAccountRepository(PaymentsDbContext ctx) => _ctx = ctx;

    public Task<bool> ExistsAsync(Guid customerId, Currency currency)
        => _ctx.WithdrawalAccounts.AnyAsync(x => x.CustomerId == customerId && x.Currency.Equals(currency));

    public Task<WithdrawalAccount> GetAsync(Guid id)
        => _ctx.WithdrawalAccounts.SingleOrDefaultAsync(x => x.Id == id);

    public Task<WithdrawalAccount> GetAsync(Guid customerId, Currency currency)
        => _ctx.WithdrawalAccounts.SingleOrDefaultAsync(x => x.CustomerId == customerId && x.Currency.Equals(currency));

    public async Task AddAsync(WithdrawalAccount withdrawalAccount)
    {
        await _ctx.WithdrawalAccounts.AddAsync(withdrawalAccount);
        await _ctx.SaveChangesAsync();
    }
}