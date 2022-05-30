using System;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Customers.Core.Clients;
using Inflow.Modules.Customers.Core.Clients.External.DTO;
using Inflow.Modules.Customers.Core.Domain.Entities;
using Inflow.Modules.Customers.Core.Domain.Repositories;
using Inflow.Modules.Customers.Core.Exceptions;
using Inflow.Shared.Abstractions.Commands;
using Inflow.Shared.Abstractions.Exceptions;
using Inflow.Shared.Abstractions.Kernel.ValueObjects;
using Inflow.Shared.Abstractions.Time;
using Microsoft.Extensions.Logging;

namespace Inflow.Modules.Customers.Core.Commands.Handlers;

internal sealed class CreateCustomerCommandHandler : ICommandHandler<CreateCustomer>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUserApiClient _userApiClient;
    private readonly ILogger<CreateCustomerCommandHandler> _logger;
    private readonly IClock _clock;

    public CreateCustomerCommandHandler(ICustomerRepository customerRepository, IUserApiClient userApiClient,
        ILogger<CreateCustomerCommandHandler> logger, IClock clock)
    {
        _customerRepository = customerRepository;
        _userApiClient = userApiClient;
        _logger = logger;
        _clock = clock;
    }

    public async Task HandleAsync(CreateCustomer command, CancellationToken cancellationToken = default)
    {
        _ = new Email(command.Email);
        var user = await _userApiClient.GetAsync(command.Email, cancellationToken);
        if (user is null)
        {
            throw ResourceNotFoundException.OfType<UserDto>(command.Email, nameof(command.Email));
        }

        if (user.Role is not "user")
        {
            return;
        }

        var customerId = user.Id;

        if (await _customerRepository.GetAsync(customerId, cancellationToken) is not null)
        {
            throw new CustomerAlreadyExistsException(customerId);
        }
        
        var customer = new Customer(customerId, command.Email, _clock.CurrentDate());
        await _customerRepository.AddAsync(customer);
        
        _logger.LogInformation("Created a customer with the id: {CustomerId}", customer.Id);
    }
}