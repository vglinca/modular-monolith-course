using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Payments.Infrastructure.Exceptions;
using Inflow.Modules.Payments.Infrastructure.Repositories;
using Inflow.Shared.Abstractions.Events;
using Microsoft.Extensions.Logging;

namespace Inflow.Modules.Payments.Core.Deposits.Events.External.Handlers;

internal sealed class CustomerLockedEventHandler : IEventHandler<CustomerLocked>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<CustomerLockedEventHandler> _logger;

    public CustomerLockedEventHandler(ICustomerRepository customerRepository, ILogger<CustomerLockedEventHandler> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task HandleAsync(CustomerLocked @event, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetAsync(@event.CustomerId);
        if (customer is null)
        {
            throw new CustomerNotFoundException(@event.CustomerId);
        }

        customer.Lock();
        await _customerRepository.UpdateAsync(customer);
        _logger.LogInformation($"Customer with ID '{@event.CustomerId}' has been locked.");
    }
}