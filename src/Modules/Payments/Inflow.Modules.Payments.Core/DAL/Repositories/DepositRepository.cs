using System;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Payments.Core.Deposits.Domain.Entities;
using Inflow.Modules.Payments.Core.Deposits.Domain.Repositories;
using Inflow.Shared.Abstractions.Exceptions;
using Inflow.Shared.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Inflow.Modules.Payments.Core.DAL.Repositories;

internal class DepositRepository : IDepositRepository
{
    private readonly PaymentsDbContext _ctx;

    public DepositRepository(PaymentsDbContext ctx) => _ctx = ctx;

    public Task<Deposit> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => _ctx.Deposits
            .Include(x => x.Account)
            .GetOneOrThrowAsync(x => x.Id.Equals(id), 
                () => throw ResourceNotFoundException.OfType<Deposit>(id), cancellationToken);

    public async Task AddAsync(Deposit deposit)
    {
        await _ctx.Deposits.AddAsync(deposit);
        await _ctx.SaveChangesAsync();
    }

    public async Task UpdateAsync(Deposit deposit)
    {
        _ctx.Deposits.Update(deposit);
        await _ctx.SaveChangesAsync();
    }
}