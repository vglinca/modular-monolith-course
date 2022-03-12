using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Customers.Core.Domain.Repositories;
using Inflow.Modules.Customers.Core.Events;
using Inflow.Modules.Customers.Core.Exceptions;
using Inflow.Shared.Abstractions.Commands;
using Inflow.Shared.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Inflow.Modules.Customers.Core.Commands.Handlers;

internal sealed class LockCustomerCommandHandler : ICommandHandler<LockCustomer>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<LockCustomerCommandHandler> _logger;

    public LockCustomerCommandHandler(ICustomerRepository customerRepository, IMessageBroker messageBroker, 
        ILogger<LockCustomerCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(LockCustomer command, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetAsync(command.CustomerId);
        if (customer is null)
        {
            throw new CustomerNotFoundException(command.CustomerId);
        }
        
        customer.Lock(command.Notes);
        await _customerRepository.UpdateAsync(customer);
        await _messageBroker.PublishAsync(new CustomerLocked(command.CustomerId), cancellationToken);
        _logger.LogInformation($"Locked a customer with ID: '{command.CustomerId}'.");
    }
}