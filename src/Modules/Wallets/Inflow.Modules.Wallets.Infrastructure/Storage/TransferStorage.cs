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

internal class TransferStorage : ITransferStorage
{
    private readonly DbSet<Transfer> _transfers;

    public TransferStorage(WalletsDbContext ctx) => _transfers = ctx.Transfers;

    public Task<Transfer> FindAsync(Expression<Func<Transfer, bool>> expression,
        CancellationToken cancellationToken = default)
        => _transfers
            .AsNoTracking()
            .AsQueryable()
            .Where(expression)
            .SingleOrDefaultAsync(cancellationToken);

    public Task<PagedResult<Transfer>> GetPagedAsync(Expression<Func<Transfer, bool>> expression,
        IPagedQuery pagedQuery, CancellationToken cancellationToken = default)
        => _transfers
            .AsNoTracking()
            .AsQueryable()
            .Where(expression)
            .OrderBy(x => x.CreatedAt)
            .PaginateAsync(pagedQuery, cancellationToken);
}