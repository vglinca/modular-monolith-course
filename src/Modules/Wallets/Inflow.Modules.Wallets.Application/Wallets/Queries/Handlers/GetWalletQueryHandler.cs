using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Wallets.Application.Wallets.DTO;
using Inflow.Modules.Wallets.Application.Wallets.Storage;
using Inflow.Modules.Wallets.Core.Wallets.Exceptions;
using Inflow.Shared.Abstractions.Queries;

namespace Inflow.Modules.Wallets.Application.Wallets.Queries.Handlers;

internal sealed class GetWalletQueryHandler : IQueryHandler<GetWallet, WalletDetailsDto>
{
    private readonly IWalletStorage _storage;

    public GetWalletQueryHandler(IWalletStorage storage) => _storage = storage;

    public async Task<WalletDetailsDto> HandleAsync(GetWallet query, CancellationToken cancellationToken = default)
    {

        var wallet = await _storage.FindAsync(x => x.Id == query.WalletId, cancellationToken);
        if (wallet is null)
        {
            throw new WalletNotFoundException(query.WalletId);
        }

        return wallet.AsDetailsDto();
    }
}