using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Payments.Core.Deposits.Domain.Entities;
using Inflow.Modules.Payments.Core.Deposits.Domain.Repositories;
using Inflow.Modules.Payments.Core.Deposits.Events;
using Inflow.Modules.Payments.Core.Deposits.Exceptions;
using Inflow.Modules.Payments.Infrastructure.Exceptions;
using Inflow.Modules.Payments.Infrastructure.Repositories;
using Inflow.Shared.Abstractions.Commands;
using Inflow.Shared.Abstractions.Exceptions;
using Inflow.Shared.Abstractions.Messaging;
using Inflow.Shared.Abstractions.Time;
using Microsoft.Extensions.Logging;

namespace Inflow.Modules.Payments.Core.Deposits.Commands.Handlers;

internal sealed class StartDepositCommandHandler : ICommandHandler<StartDeposit>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IDepositRepository _depositRepository;
    private readonly IDepositAccountRepository _depositAccountRepository;
    private readonly IClock _clock;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<StartDepositCommandHandler> _logger;

    public StartDepositCommandHandler(ICustomerRepository customerRepository, IDepositRepository depositRepository, 
        IDepositAccountRepository depositAccountRepository, IClock clock, IMessageBroker messageBroker, ILogger<StartDepositCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _depositRepository = depositRepository;
        _depositAccountRepository = depositAccountRepository;
        _clock = clock;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(StartDeposit command, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetAsync(command.CustomerId, cancellationToken);

        if (!customer.IsActive || !customer.IsVerified)
        {
            throw new CustomerNotActiveException(command.CustomerId);
        }

        var account = await _depositAccountRepository.GetAsync(command.CustomerId, command.Currency, cancellationToken);
        if (account is null)
        {
            throw new ResourceNotFoundException(
                $"Deposit Account for a Customer with ID: '{command.CustomerId}' was not found");
        }

        var deposit = account.CreateDeposit(command.DepositId, command.Amount, _clock.CurrentDate());
        await _depositRepository.AddAsync(deposit);
        await _messageBroker.PublishAsync(new DepositStarted(deposit.Id, account.CustomerId, deposit.Currency,
            deposit.Amount), cancellationToken);
        
        _logger.LogInformation($"Started a deposit with ID: '{command.DepositId}'.");
    }
}