using System;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Payments.Infrastructure.Entities;
using Inflow.Modules.Payments.Infrastructure.Repositories;
using Inflow.Shared.Abstractions.Events;
using Microsoft.Extensions.Logging;

namespace Inflow.Modules.Payments.Core.Deposits.Events.External.Handlers;

internal sealed class CustomerCompletedEventHandler : IEventHandler<CustomerCompleted>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<CustomerCompletedEventHandler> _logger;

    public CustomerCompletedEventHandler(ICustomerRepository customerRepository, ILogger<CustomerCompletedEventHandler> logger)
    {
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task HandleAsync(CustomerCompleted @event, CancellationToken cancellationToken = default)
    {
        var customer = new Customer(@event.CustomerId, @event.FullName, @event.Nationality);
        await _customerRepository.AddAsync(customer);
        _logger.LogInformation($"Created customer with id: {customer.Id}");
    }
}