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

internal sealed class WithdrawalStartedEventHandler : IEventHandler<WithdrawalStarted>
{
    private const string TransferName = "withdrawal";
    private readonly IWalletRepository _walletRepository;
    private readonly IClock _clock;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<WithdrawalStartedEventHandler> _logger;

    public WithdrawalStartedEventHandler(IWalletRepository walletRepository, IClock clock, IMessageBroker messageBroker,
        ILogger<WithdrawalStartedEventHandler> logger)
    {
        _walletRepository = walletRepository;
        _clock = clock;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(WithdrawalStarted @event, CancellationToken cancellationToken = default)
    {
        var wallet = await _walletRepository.GetAsync(@event.CustomerId, @event.Currency);
        if (wallet is null)
        {
            throw new ResourceNotFoundException(
                $"Wallet for customer with ID '{@event.CustomerId}' was not found");
        }

        try
        {
            var transfer = wallet.DeductFunds(Guid.NewGuid(), @event.Amount, _clock.CurrentDate(), TransferName,
                GetMetadata(@event.WithdrawalId));
            await _messageBroker.PublishAsync(new FundsDeducted(wallet.Id, wallet.OwnerId, wallet.Currency,
                @event.Amount, transfer.Name, transfer.Metadata), cancellationToken);
            _logger.LogInformation($"Deducted {@event.Amount} {wallet.Currency} from wallet with ID: '{wallet.Id}'" +
                                   $"based on started withdrawal with ID: '{@event.WithdrawalId}'.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            _logger.LogError($"Couldn't deduct {@event.Amount} {wallet.Currency} from wallet with ID: '{wallet.Id}'" +
                             $"based on started withdrawal with ID: '{@event.WithdrawalId}'.");
            await _messageBroker.PublishAsync(new DeductFundsRejected(wallet.Id, wallet.OwnerId, wallet.Currency,
                @event.Amount, TransferName, GetMetadata(@event.WithdrawalId)), cancellationToken);
        }
        finally
        {
            await _walletRepository.UpdateAsync(wallet);
        }
    }
    
    private static string GetMetadata(Guid withdrawalId) => $"{{\"withdrawalId\": \"{withdrawalId}\"}}";
}