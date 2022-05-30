using System;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Payments.Core.Deposits.Domain.Entities;
using Inflow.Modules.Payments.Core.Deposits.Domain.Repositories;
using Inflow.Shared.Abstractions.Exceptions;
using Inflow.Shared.Abstractions.Kernel.ValueObjects;
using Inflow.Shared.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Inflow.Modules.Payments.Core.DAL.Repositories;

internal class DepositAccountRepository : IDepositAccountRepository
{
    private readonly PaymentsDbContext _ctx;

    public DepositAccountRepository(PaymentsDbContext ctx) => _ctx = ctx;

    public Task<DepositAccount> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => _ctx.DepositAccounts
            .GetOneOrThrowAsync(x => x.Id.Equals(id), 
                () => throw ResourceNotFoundException.OfType<DepositAccount>(id), cancellationToken);

    public Task<DepositAccount> GetAsync(Guid customerId, Currency currency,
        CancellationToken cancellationToken = default)
        => _ctx.DepositAccounts.SingleOrDefaultAsync(x => x.CustomerId == customerId && x.Currency.Equals(currency),
            cancellationToken: cancellationToken);

    public async Task AddAsync(DepositAccount depositAccount)
    {
        await _ctx.DepositAccounts.AddAsync(depositAccount);
        await _ctx.SaveChangesAsync();
    }
}