using System.Threading.Tasks;
using Inflow.Modules.Wallets.Core.Owners.Entities;
using Inflow.Modules.Wallets.Core.Owners.Repositories;
using Inflow.Modules.Wallets.Core.Owners.Types;
using Microsoft.EntityFrameworkCore;

namespace Inflow.Modules.Wallets.Infrastructure.EF.Repositories;

internal class IndividualOwnerRepository : IIndividualOwnerRepository
{
    private readonly WalletsDbContext _ctx;

    public IndividualOwnerRepository(WalletsDbContext ctx) => _ctx = ctx;

    public Task<IndividualOwner> GetAsync(OwnerId id)
        => _ctx.IndividualOwners.SingleOrDefaultAsync(x => x.Id == id);

    public async Task AddAsync(IndividualOwner owner)
    {
        await _ctx.IndividualOwners.AddAsync(owner);
        await _ctx.SaveChangesAsync();
    }

    public async Task UpdateAsync(IndividualOwner owner)
    {
        _ctx.IndividualOwners.Update(owner);
        await _ctx.SaveChangesAsync();
    }
}