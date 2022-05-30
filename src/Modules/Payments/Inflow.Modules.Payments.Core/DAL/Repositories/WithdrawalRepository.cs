using System;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Payments.Core.Withdrawals.Domain.Entities;
using Inflow.Modules.Payments.Core.Withdrawals.Domain.Repositories;
using Inflow.Shared.Abstractions.Exceptions;
using Inflow.Shared.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Inflow.Modules.Payments.Core.DAL.Repositories;

internal class WithdrawalRepository : IWithdrawalRepository
{
    private readonly PaymentsDbContext _ctx;

    public WithdrawalRepository(PaymentsDbContext ctx) => _ctx = ctx;

    public Task<Withdrawal> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => _ctx.Withdrawals
            .Include(x => x.Account)
            .GetOneOrThrowAsync(x => x.Id.Equals(id), 
                () => ResourceNotFoundException.OfType<Withdrawal>(id), cancellationToken);

    public async Task AddAsync(Withdrawal withdrawal)
    {
        await _ctx.Withdrawals.AddAsync(withdrawal);
        await _ctx.SaveChangesAsync();
    }

    public async Task UpdateAsync(Withdrawal withdrawal)
    {
        _ctx.Withdrawals.Update(withdrawal);
        await _ctx.SaveChangesAsync();
    }
}