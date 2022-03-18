using System;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Wallets.Core.Wallets.Entities;
using Inflow.Modules.Wallets.Core.Wallets.Repositories;
using Inflow.Shared.Abstractions.Events;
using Inflow.Shared.Abstractions.Messaging;
using Inflow.Shared.Abstractions.Time;
using Microsoft.Extensions.Logging;

namespace Inflow.Modules.Wallets.Application.Wallets.Events.External.Handlers;

internal sealed class DepositAccountAddedEventHandler : IEventHandler<DepositAccountAdded>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IClock _clock;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<DepositAccountAddedEventHandler> _logger;

    public DepositAccountAddedEventHandler(IWalletRepository walletRepository, IClock clock, 
        IMessageBroker messageBroker, ILogger<DepositAccountAddedEventHandler> logger)
    {
        _walletRepository = walletRepository;
        _clock = clock;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(DepositAccountAdded @event, CancellationToken cancellationToken = default)
    {
        var wallet = new Wallet(Guid.NewGuid(), @event.CustomerId, @event.Currency, _clock.CurrentDate());
        await _walletRepository.AddAsync(wallet);
        await _messageBroker.PublishAsync(new WalletAdded(wallet.Id, wallet.OwnerId, wallet.Currency),
            cancellationToken);
        
        _logger.LogInformation("Created a new wallet with ID: '{WalletId}', owner ID: '{OwnerId}', " +
                               "currency: '{Currency}' for deposit account with ID: '{AccountId}'.", 
            wallet.Id, wallet.OwnerId, @event.Currency, @event.AccountId);
    }
}