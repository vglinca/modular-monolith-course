using System;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Wallets.Core.Wallets.Exceptions;
using Inflow.Modules.Wallets.Core.Wallets.Repositories;
using Inflow.Shared.Abstractions.Events;
using Inflow.Shared.Abstractions.Exceptions;
using Inflow.Shared.Abstractions.Messaging;
using Inflow.Shared.Abstractions.Time;
using Microsoft.Extensions.Logging;

namespace Inflow.Modules.Wallets.Application.Wallets.Events.External.Handlers;

internal sealed class DepositCompletedEventHandler : IEventHandler<DepositCompleted>
{
    private const string TransferName = "deposit";
    private readonly IWalletRepository _walletRepository;
    private readonly IClock _clock;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<DepositCompletedEventHandler> _logger;

    public DepositCompletedEventHandler(IWalletRepository walletRepository, IClock clock, IMessageBroker messageBroker,
        ILogger<DepositCompletedEventHandler> logger)
    {
        _walletRepository = walletRepository;
        _clock = clock;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(DepositCompleted @event, CancellationToken cancellationToken = default)
    {
        var wallet = await _walletRepository.GetAsync(@event.CustomerId, @event.Currency);
        if (wallet is null)
        {
            throw new ResourceNotFoundException(
                $"Wallet for customer with ID '{@event.CustomerId}' was not found");
        }

        var transfer = wallet.AddFunds(Guid.NewGuid(), @event.Amount, _clock.CurrentDate(), TransferName,
            GetMetadata(@event.DepositId));
        await _walletRepository.UpdateAsync(wallet);
        await _messageBroker.PublishAsync(
            new FundsAdded(wallet.Id, wallet.OwnerId, wallet.Currency, transfer.Amount, transfer.Name,
                transfer.Metadata), cancellationToken);
        
        _logger.LogInformation($"Added {@event.Amount} {wallet.Currency} to wallet with ID: '{wallet.Id}'" +
                               $"based on completed deposit with ID: '{@event.DepositId}'.");
    }
    
    private static string GetMetadata(Guid depositId) => $"{{\"depositId\": \"{depositId}\"}}";
}