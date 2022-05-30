using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Customers.Core.Domain.Repositories;
using Inflow.Modules.Customers.Core.Events;
using Inflow.Modules.Customers.Core.Exceptions;
using Inflow.Shared.Abstractions.Commands;
using Inflow.Shared.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Inflow.Modules.Customers.Core.Commands.Handlers;

internal sealed class UnlockCustomerCommandHandler : ICommandHandler<UnlockCustomer>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<UnlockCustomerCommandHandler> _logger;

    public UnlockCustomerCommandHandler(ICustomerRepository customerRepository, ILogger<UnlockCustomerCommandHandler> logger, 
        IMessageBroker messageBroker)
    {
        _customerRepository = customerRepository;
        _logger = logger;
        _messageBroker = messageBroker;
    }

    public async Task HandleAsync(UnlockCustomer command, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetAsync(command.CustomerId, cancellationToken);

        customer.Unlock(command.Notes);
        await _customerRepository.UpdateAsync(customer);
        await _messageBroker.PublishAsync(new CustomerUnlocked(command.CustomerId), cancellationToken);
        
        _logger.LogInformation("Unlocked a customer with ID: '{CustomerId}'.", command.CustomerId);
    }
}