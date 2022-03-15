using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Wallets.Application.Wallets.DTO;
using Inflow.Modules.Wallets.Application.Wallets.Storage;
using Inflow.Modules.Wallets.Core.Wallets.Entities;
using Inflow.Shared.Abstractions.Kernel.ValueObjects;
using Inflow.Shared.Abstractions.Queries;

namespace Inflow.Modules.Wallets.Application.Wallets.Queries.Handlers;

internal sealed class BrowseTransfersQueryHandler : IQueryHandler<BrowseTransfers, PagedResult<TransferDto>>
{
    private readonly ITransferStorage _storage;

    public BrowseTransfersQueryHandler(ITransferStorage storage) => _storage = storage;

    public async Task<PagedResult<TransferDto>> HandleAsync(BrowseTransfers query, CancellationToken cancellationToken = default)
    {
        var expression = GetFilterExpression(query);
        var result = await _storage.GetPagedAsync(expression, query, cancellationToken);
        var transfers = result.Items.Select(x => x.AsDto()).ToList();
        
        return PagedResult<TransferDto>.From(result, transfers);
    }

    private static Expression<Func<Transfer, bool>> GetFilterExpression(BrowseTransfers query)
    {
        Expression<Func<Transfer, bool>> expression = x => true;
        if (!string.IsNullOrWhiteSpace(query.Currency))
        {
            _ = new Currency(query.Currency);
            expression = expression.And(x => x.Currency == query.Currency);
        }

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            expression = expression.And(x => x.Name == query.Name);
        }

        return expression;
    }
}