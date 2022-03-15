using System;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Wallets.Application.Wallets.DTO;
using Inflow.Modules.Wallets.Application.Wallets.Storage;
using Inflow.Modules.Wallets.Core.Wallets.Exceptions;
using Inflow.Shared.Abstractions.Queries;

namespace Inflow.Modules.Wallets.Application.Wallets.Queries.Handlers;

internal sealed class GetTransferQueryHandler : IQueryHandler<GetTransfer, TransferDetailsDto>
{
    private readonly ITransferStorage _storage;

    public GetTransferQueryHandler(ITransferStorage storage) => _storage = storage;

    public async Task<TransferDetailsDto> HandleAsync(GetTransfer query, CancellationToken cancellationToken = default)
    {
        var transfer = await _storage.FindAsync(x => x.Id == query.TransferId, cancellationToken);
        if (transfer is null)
        {
            throw new TransferNotFoundException((Guid) query.TransferId);
        }

        return transfer.AsDetailsDto();
    }
}