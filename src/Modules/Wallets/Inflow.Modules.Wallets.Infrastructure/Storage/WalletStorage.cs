using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Wallets.Application.Wallets.Storage;
using Inflow.Modules.Wallets.Core.Wallets.Entities;
using Inflow.Modules.Wallets.Infrastructure.EF;
using Inflow.Shared.Abstractions.Queries;
using Inflow.Shared.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Inflow.Modules.Wallets.Infrastructure.Storage;

internal class WalletStorage : IWalletStorage
{
    private readonly DbSet<Wallet> _wallets;

    public WalletStorage(WalletsDbContext ctx) => _wallets = ctx.Wallets;

    public Task<Wallet> FindAsync(Expression<Func<Wallet, bool>> expression,
        CancellationToken cancellationToken = default)
        => _wallets
            .AsNoTracking()
            .Include(x => x.Transfers)
            .AsQueryable()
            .Where(expression)
            .SingleOrDefaultAsync(cancellationToken);

    public Task<PagedResult<Wallet>> GetPagedAsync(Expression<Func<Wallet, bool>> expression, IPagedQuery query,
        CancellationToken cancellationToken = default)
        => _wallets
            .AsNoTracking()
            .AsQueryable()
            .Where(expression)
            .OrderBy(x => x.CreatedAt)
            .PaginateAsync(query, cancellationToken);
}