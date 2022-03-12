using System;
using System.Threading.Tasks;
using Inflow.Modules.Payments.Core.Withdrawals.Domain.Entities;
using Inflow.Modules.Payments.Core.Withdrawals.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Inflow.Modules.Payments.Core.DAL.Repositories;

internal class WithdrawalRepository : IWithdrawalRepository
{
    private readonly PaymentsDbContext _ctx;

    public WithdrawalRepository(PaymentsDbContext ctx) => _ctx = ctx;

    public Task<Withdrawal> GetAsync(Guid id)
        => _ctx.Withdrawals
            .Include(x => x.Account)
            .SingleOrDefaultAsync(x => x.Id == id);

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