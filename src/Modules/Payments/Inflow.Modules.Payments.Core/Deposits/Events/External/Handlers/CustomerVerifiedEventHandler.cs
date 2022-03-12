using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Payments.Core.Deposits.Domain.Factories;
using Inflow.Modules.Payments.Core.Deposits.Domain.Repositories;
using Inflow.Modules.Payments.Core.Deposits.Domain.Services;
using Inflow.Modules.Payments.Infrastructure.Exceptions;
using Inflow.Modules.Payments.Infrastructure.Repositories;
using Inflow.Shared.Abstractions.Events;
using Inflow.Shared.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Inflow.Modules.Payments.Core.Deposits.Events.External.Handlers;

internal sealed class CustomerVerifiedEventHandler : IEventHandler<CustomerVerified>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IDepositAccountRepository _depositAccountRepository;
    private readonly IDepositAccountFactory _depositAccountFactory;
    private readonly ICurrencyResolver _currencyResolver;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<CustomerVerifiedEventHandler> _logger;

    public CustomerVerifiedEventHandler(ICustomerRepository customerRepository, 
        IDepositAccountRepository depositAccountRepository, IDepositAccountFactory depositAccountFactory,
        ICurrencyResolver currencyResolver, IMessageBroker messageBroker, ILogger<CustomerVerifiedEventHandler> logger)
    {
        _customerRepository = customerRepository;
        _depositAccountRepository = depositAccountRepository;
        _depositAccountFactory = depositAccountFactory;
        _currencyResolver = currencyResolver;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(CustomerVerified @event, CancellationToken cancellationToken = default)
    {
        var customer = await _customerRepository.GetAsync(@event.CustomerId);
        if (customer is null)
        {
            throw new CustomerNotFoundException(@event.CustomerId);
        }
        
        customer.Verify();
        await _customerRepository.UpdateAsync(customer);
        var currency = _currencyResolver.GetForNationality(customer.Nationality);
        var account = _depositAccountFactory.Create(customer.Id, customer.Nationality, currency);
        await _depositAccountRepository.AddAsync(account);
        await _messageBroker.PublishAsync(new DepositAccountAdded(account.Id, customer.Id, currency),
            cancellationToken);
        
        _logger.LogInformation($"Added a deposit account with ID: '{account.Id}' " +
                               $"for customer with ID: '{account.CustomerId}'.");
    }
}