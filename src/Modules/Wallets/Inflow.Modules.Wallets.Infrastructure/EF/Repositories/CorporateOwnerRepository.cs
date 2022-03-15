using System.Threading.Tasks;
using Inflow.Modules.Wallets.Core.Owners.Entities;
using Inflow.Modules.Wallets.Core.Owners.Repositories;
using Inflow.Modules.Wallets.Core.Owners.Types;
using Microsoft.EntityFrameworkCore;

namespace Inflow.Modules.Wallets.Infrastructure.EF.Repositories;

internal class CorporateOwnerRepository : ICorporateOwnerRepository
{
    private readonly WalletsDbContext _ctx;

    public CorporateOwnerRepository(WalletsDbContext ctx) => _ctx = ctx;

    public Task<CorporateOwner> GetAsync(OwnerId id)
        => _ctx.CorporateOwners.SingleOrDefaultAsync(x => x.Id == id);

    public async Task AddAsync(CorporateOwner owner)
    {
        await _ctx.CorporateOwners.AddAsync(owner);
        await _ctx.SaveChangesAsync();
    }

    public async Task UpdateAsync(CorporateOwner owner)
    {
        _ctx.CorporateOwners.Update(owner);
        await _ctx.SaveChangesAsync();
    }
}