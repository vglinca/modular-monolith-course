using System;
using System.Threading.Tasks;
using Inflow.Modules.Payments.Core.Deposits.Domain.Entities;
using Inflow.Modules.Payments.Core.Deposits.Domain.Repositories;
using Inflow.Shared.Abstractions.Kernel.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Inflow.Modules.Payments.Core.DAL.Repositories;

internal class DepositAccountRepository : IDepositAccountRepository
{
    private readonly PaymentsDbContext _ctx;

    public DepositAccountRepository(PaymentsDbContext ctx) => _ctx = ctx;

    public Task<DepositAccount> GetAsync(Guid id)
        => _ctx.DepositAccounts.SingleOrDefaultAsync(x => x.Id == id);

    public Task<DepositAccount> GetAsync(Guid customerId, Currency currency)
        => _ctx.DepositAccounts.SingleOrDefaultAsync(x => x.CustomerId == customerId && x.Currency.Equals(currency));

    public async Task AddAsync(DepositAccount depositAccount)
    {
        await _ctx.DepositAccounts.AddAsync(depositAccount);
        await _ctx.SaveChangesAsync();
    }
}