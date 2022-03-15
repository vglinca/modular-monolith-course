using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Wallets.Core.Wallets.Entities;
using Inflow.Shared.Abstractions.Queries;

namespace Inflow.Modules.Wallets.Application.Wallets.Storage;

internal interface IWalletStorage
{
    Task<Wallet> FindAsync(Expression<Func<Wallet, bool>> expression, CancellationToken cancellationToken = default);
    Task<PagedResult<Wallet>> GetPagedAsync(Expression<Func<Wallet, bool>> expression, IPagedQuery query,
        CancellationToken cancellationToken = default);
}