using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Payments.Infrastructure.Exceptions;
using Inflow.Modules.Payments.Infrastructure.Repositories;
using Inflow.Shared.Abstractions.Events;
using Microsoft.Extensions.Logging;

namespace Inflow.Modules.Payments.Core.Deposits.Events.External.Handlers;

internal sealed class CustomerUnlockedEventHandler : IEventHandler<CustomerUnlocked>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<CustomerUnlockedEventHandler> _logger;

    public CustomerUnlockedEventHandler(ICustomerRepository customerRepository, ILogger<CustomerUnlockedEventHandler> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task HandleAsync(CustomerUnlocked @event, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetAsync(@event.CustomerId);
        if (customer is null)
        {
            throw new CustomerNotFoundException(@event.CustomerId);
        }
            
        customer.Unlock();
        await _customerRepository.UpdateAsync(customer);
        _logger.LogInformation($"Customer with ID '{@event.CustomerId}' has been unlocked.");
    }
}