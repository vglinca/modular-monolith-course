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

internal sealed class AddFundsCommandHAndler : ICommandHandler<AddFunds>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IMessageBroker _messageBroker;
    private readonly IClock _clock;
    private readonly ILogger<AddFundsCommandHAndler> _logger;

    public AddFundsCommandHAndler(IWalletRepository walletRepository, IMessageBroker messageBroker, IClock clock, 
        ILogger<AddFundsCommandHAndler> logger)
    {
        _walletRepository = walletRepository;
        _messageBroker = messageBroker;
        _clock = clock;
        _logger = logger;
    }

    public async Task HandleAsync(AddFunds command, CancellationToken cancellationToken = default)
    {
        var wallet = await _walletRepository.GetAsync(command.WalletId);
        if (wallet is null)
        {
            throw new WalletNotFoundException(command.WalletId);
        }

        if (wallet.Currency != command.Currency)
        {
            throw new InvalidTransferCurrencyException(command.Currency);
        }

        var transfer = wallet.AddFunds(command.TransferId, command.Amount, _clock.CurrentDate(), command.TransferName,
            command.TransferMetadata);

        await _walletRepository.UpdateAsync(wallet);
        await _messageBroker.PublishAsync(new FundsAdded(wallet.Id, wallet.OwnerId, wallet.Currency,
            transfer.Amount, transfer.Name, transfer.Metadata), cancellationToken);
        
        _logger.LogInformation($"Added {transfer.Amount} {transfer.Currency} to wallet with ID: '{wallet.Id}'.");
    }
}