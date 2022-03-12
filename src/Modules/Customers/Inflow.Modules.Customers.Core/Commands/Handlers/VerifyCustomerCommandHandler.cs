using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Customers.Core.Domain.Repositories;
using Inflow.Modules.Customers.Core.Events;
using Inflow.Modules.Customers.Core.Exceptions;
using Inflow.Shared.Abstractions.Commands;
using Inflow.Shared.Abstractions.Messaging;
using Inflow.Shared.Abstractions.Time;
using Microsoft.Extensions.Logging;

namespace Inflow.Modules.Customers.Core.Commands.Handlers;

internal sealed class VerifyCustomerCommandHandler : ICommandHandler<VerifyCustomer>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMessageBroker _messageBroker;
    private readonly IClock _clock;
    private readonly ILogger<VerifyCustomerCommandHandler> _logger;

    public VerifyCustomerCommandHandler(ICustomerRepository customerRepository, IMessageBroker messageBroker, 
        IClock clock, ILogger<VerifyCustomerCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _messageBroker = messageBroker;
        _clock = clock;
        _logger = logger;
    }

    public async Task HandleAsync(VerifyCustomer command, CancellationToken cancellationToken = default)
    {
        
        var customer = await _customerRepository.GetAsync(command.CustomerId);
        if (customer is null)
        {
            throw new CustomerNotFoundException(command.CustomerId);
        }
        
        customer.Verify(_clock.CurrentDate());
        await _customerRepository.UpdateAsync(customer);
        await _messageBroker.PublishAsync(new CustomerVerified(command.CustomerId), cancellationToken);
        _logger.LogInformation($"Verified a customer with ID: '{command.CustomerId}'.");
    }
}