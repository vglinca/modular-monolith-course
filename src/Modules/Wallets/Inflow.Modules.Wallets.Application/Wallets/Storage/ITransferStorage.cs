using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Wallets.Core.Wallets.Entities;
using Inflow.Shared.Abstractions.Queries;

namespace Inflow.Modules.Wallets.Application.Wallets.Storage;

internal interface ITransferStorage
{
    Task<Transfer> FindAsync(Expression<Func<Transfer, bool>> expression, CancellationToken cancellationToken = default);
    Task<PagedResult<Transfer>> GetPagedAsync(Expression<Func<Transfer, bool>> expression, IPagedQuery pagedQuery, 
        CancellationToken cancellationToken = default);
}