using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Wallets.Application.Wallets.Events;
using Inflow.Modules.Wallets.Core.Wallets.Exceptions;
using Inflow.Modules.Wallets.Core.Wallets.Repositories;
using Inflow.Shared.Abstractions.Commands;
using Inflow.Shared.Abstractions.Messaging;
using Inflow.Shared.Abstractions.Time;
using Microsoft.Extensions.Logging;

namespace Inflow.Modules.Wallets.Application.Wallets.Commands.Handlers;

internal sealed class DeductFundsCommandHandler : ICommandHandler<DeductFunds>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IClock _clock;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<DeductFundsCommandHandler> _logger;

    public DeductFundsCommandHandler(IWalletRepository walletRepository, IClock clock, IMessageBroker messageBroker,
        ILogger<DeductFundsCommandHandler> logger)
    {
        _walletRepository = walletRepository;
        _clock = clock;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(DeductFunds command, CancellationToken cancellationToken = default)
    {
        var wallet = await _walletRepository.GetAsync(command.WalletId, cancellationToken);

        if (wallet.Currency != command.Currency)
        {
            throw new InvalidTransferCurrencyException(command.Currency);
        }

        var transfer = wallet.DeductFunds(command.TransferId, command.Amount, _clock.CurrentDate(),
            command.TransferName, command.TransferMetadata);
        
        await _walletRepository.UpdateAsync(wallet);
        await _messageBroker.PublishAsync(new FundsDeducted(wallet.Id, wallet.OwnerId, wallet.Currency,
            command.Amount, transfer.Name, transfer.Metadata), cancellationToken);
        _logger.LogInformation($"Deducted {transfer.Amount} {transfer.Currency} from wallet with ID: '{wallet.Id}'.");
    }
}