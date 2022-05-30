using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Wallets.Application.Wallets.Events;
using Inflow.Modules.Wallets.Core.Wallets.Entities;
using Inflow.Modules.Wallets.Core.Wallets.Exceptions;
using Inflow.Modules.Wallets.Core.Wallets.Repositories;
using Inflow.Shared.Abstractions.Commands;
using Inflow.Shared.Abstractions.Kernel.ValueObjects;
using Inflow.Shared.Abstractions.Messaging;
using Inflow.Shared.Abstractions.Time;
using Microsoft.Extensions.Logging;

namespace Inflow.Modules.Wallets.Application.Wallets.Commands.Handlers;

internal sealed class TransferFundsCommandHandler : ICommandHandler<TransferFunds>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IClock _clock;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<TransferFundsCommandHandler> _logger;

    public TransferFundsCommandHandler(IWalletRepository walletRepository, IClock clock, IMessageBroker messageBroker,
        ILogger<TransferFundsCommandHandler> logger)
    {
        _walletRepository = walletRepository;
        _clock = clock;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(TransferFunds command, CancellationToken cancellationToken = default)
    {
        var amount = new Amount(command.Amount);
        
        var ownerWallet = await _walletRepository.GetAsync(command.OwnerWalletId, cancellationToken);

        if (ownerWallet.Currency != command.Currency)
        {
            throw new InvalidTransferCurrencyException(command.Currency);
        }

        var receiverWallet = await _walletRepository.GetAsync(command.ReceiverWalletId, cancellationToken);

        if (receiverWallet.Currency != command.Currency)
        {
            throw new InvalidTransferCurrencyException(command.Currency);
        }

        var now = _clock.CurrentDate();
        var transfers = ownerWallet.TransferFunds(receiverWallet, amount, now);

        var outgoingTransfer = transfers.OfType<OutgoingTransfer>().Single();
        var incomingTransfer = transfers.OfType<IncomingTransfer>().Single();

        await _walletRepository.UpdateAsync(ownerWallet);
        await _walletRepository.UpdateAsync(receiverWallet);

        await _messageBroker.PublishAsync(new IMessage[]
        {
            new FundsAdded(receiverWallet.Id, receiverWallet.OwnerId, receiverWallet.Currency, incomingTransfer.Amount,
                incomingTransfer.Name, incomingTransfer.Metadata),
            new FundsDeducted(ownerWallet.Id, ownerWallet.OwnerId, ownerWallet.Currency, outgoingTransfer.Amount,
                outgoingTransfer.Name, outgoingTransfer.Metadata)
        }, cancellationToken);

        _logger.LogInformation($"Transferred {outgoingTransfer.Amount} {outgoingTransfer.Currency}" +
                               $"from wallet with ID: '{ownerWallet.Id}' to wallet with ID: '{receiverWallet.Id}'.");
    }
}