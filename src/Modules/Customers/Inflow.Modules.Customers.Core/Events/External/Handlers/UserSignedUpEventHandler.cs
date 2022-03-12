using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Customers.Core.Domain.Entities;
using Inflow.Modules.Customers.Core.Domain.Repositories;
using Inflow.Modules.Users.Core.Exceptions;
using Inflow.Shared.Abstractions.Events;
using Inflow.Shared.Abstractions.Messaging;
using Inflow.Shared.Abstractions.Time;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Inflow.Modules.Customers.Core.Events.External.Handlers;

internal sealed class UserSignedUpEventHandler : IEventHandler<UserSignedUp>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<UserSignedUpEventHandler> _logger;
    private readonly IMessageBroker _messageBroker;
    private readonly IClock _clock;

    public UserSignedUpEventHandler(ICustomerRepository customerRepository, ILogger<UserSignedUpEventHandler> logger,
        IClock clock, IMessageBroker messageBroker)
    {
        _customerRepository = customerRepository;
        _logger = logger;
        _clock = clock;
        _messageBroker = messageBroker;
    }
    public async Task HandleAsync(UserSignedUp @event, CancellationToken cancellationToken = default)
    {
        if (@event.Role is not "user")
        {
            return;
        }

        var customerId = @event.UserId;

        if (await _customerRepository.GetAsync(customerId) is not null)
        {
            throw new CustomerAlreadyExistsException(customerId);
        }
        
        var customer = new Customer(customerId, @event.Email, _clock.CurrentDate());
        await _customerRepository.AddAsync(customer);
        
        _logger.LogInformation($"Created a customer with the id: {customer.Id}");

        await _messageBroker.PublishAsync(new CustomerCreated(customer.Id), cancellationToken);
    }
}