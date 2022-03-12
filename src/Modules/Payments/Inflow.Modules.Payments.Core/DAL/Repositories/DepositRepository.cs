using System;
using System.Threading.Tasks;
using Inflow.Modules.Payments.Core.Deposits.Domain.Entities;
using Inflow.Modules.Payments.Core.Deposits.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Inflow.Modules.Payments.Core.DAL.Repositories;

internal class DepositRepository : IDepositRepository
{
    private readonly PaymentsDbContext _ctx;

    public DepositRepository(PaymentsDbContext ctx) => _ctx = ctx;

    public Task<Deposit> GetAsync(Guid id)
        => _ctx.Deposits
            .Include(x => x.Account)
            .SingleOrDefaultAsync(x => x.Id == id);

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