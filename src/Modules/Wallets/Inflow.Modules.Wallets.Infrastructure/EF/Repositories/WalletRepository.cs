using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Wallets.Core.Owners.Types;
using Inflow.Modules.Wallets.Core.Wallets.Entities;
using Inflow.Modules.Wallets.Core.Wallets.Repositories;
using Inflow.Modules.Wallets.Core.Wallets.Types;
using Inflow.Shared.Abstractions.Exceptions;
using Inflow.Shared.Abstractions.Kernel.ValueObjects;
using Inflow.Shared.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Inflow.Modules.Wallets.Infrastructure.EF.Repositories;

internal class WalletRepository : IWalletRepository
{
    private readonly WalletsDbContext _ctx;

    public WalletRepository(WalletsDbContext ctx) => _ctx = ctx;

    public Task<Wallet> GetAsync(WalletId id, CancellationToken cancellationToken = default)
        => _ctx.Wallets
            .Include(x => x.Transfers)
            .GetOneOrThrowAsync(x => x.Id.Equals(id),
                () => throw ResourceNotFoundException.OfType<Wallet>(id), cancellationToken);

    public Task<Wallet> GetAsync(OwnerId ownerId, Currency currency)
        => _ctx.Wallets
            .Include(x => x.Transfers)
            .SingleOrDefaultAsync(x => x.OwnerId == ownerId && x.Currency.Equals(currency));

    public async Task AddAsync(Wallet wallet)
    {
        await _ctx.Wallets.AddAsync(wallet);
        await _ctx.SaveChangesAsync();
    }

    public async Task UpdateAsync(Wallet wallet)
    {
        _ctx.Wallets.Update(wallet);
        await _ctx.SaveChangesAsync();
    }
}