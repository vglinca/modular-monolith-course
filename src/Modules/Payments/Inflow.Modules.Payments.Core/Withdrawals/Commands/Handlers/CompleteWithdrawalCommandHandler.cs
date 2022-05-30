using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Payments.Core.Withdrawals.Domain.Entities;
using Inflow.Modules.Payments.Core.Withdrawals.Domain.Repositories;
using Inflow.Modules.Payments.Core.Withdrawals.Events;
using Inflow.Modules.Payments.Core.Withdrawals.Exceptions;
using Inflow.Shared.Abstractions.Commands;
using Inflow.Shared.Abstractions.Events;
using Inflow.Shared.Abstractions.Messaging;
using Inflow.Shared.Abstractions.Time;
using Microsoft.Extensions.Logging;

namespace Inflow.Modules.Payments.Core.Withdrawals.Commands.Handlers;

internal sealed class CompleteWithdrawalCommandHandler : ICommandHandler<CompleteWithdrawal>
{
    private readonly IWithdrawalRepository _withdrawalRepository;
    private readonly IClock _clock;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<CompleteWithdrawalCommandHandler> _logger;

    public CompleteWithdrawalCommandHandler(IWithdrawalRepository withdrawalRepository, IClock clock, 
        IMessageBroker messageBroker, ILogger<CompleteWithdrawalCommandHandler> logger)
    {
        _withdrawalRepository = withdrawalRepository;
        _clock = clock;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(CompleteWithdrawal command, CancellationToken cancellationToken = default)
    {
        var withdrawal = await _withdrawalRepository.GetAsync(command.WithdrawalId);

        _logger.LogInformation($"Started processing a withdrawal with ID: '{command.WithdrawalId}'...");
        var (isCompleted, @event) = TryComplete(withdrawal, command.Secret);
        var now = _clock.CurrentDate();
        if (isCompleted)
        {
            withdrawal.Complete(now);
        }
        else
        {
            withdrawal.Reject(now);
        }

        await _withdrawalRepository.UpdateAsync(withdrawal);
        await _messageBroker.PublishAsync(@event, cancellationToken);
        _logger.LogInformation($"{(isCompleted ? "Completed" : "Rejected")} " +
                               $"a withdrawal with ID: '{command.WithdrawalId}'.");
    }

    private static (bool isCompleted, IEvent @event) TryComplete(Withdrawal withdrawal, string secret) =>
        secret == "secret"
            ? (true, new WithdrawalCompleted(withdrawal.Id, withdrawal.Account.CustomerId,
                withdrawal.Account.Currency, withdrawal.Amount))
            : (false, new WithdrawalRejected(withdrawal.Id, withdrawal.Account.CustomerId,
                withdrawal.Account.Currency, withdrawal.Amount));
}