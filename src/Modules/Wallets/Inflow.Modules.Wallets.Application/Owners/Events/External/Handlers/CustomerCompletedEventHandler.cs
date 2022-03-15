using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Wallets.Core.Owners.Entities;
using Inflow.Modules.Wallets.Core.Owners.Repositories;
using Inflow.Shared.Abstractions.Events;
using Inflow.Shared.Abstractions.Time;
using Microsoft.Extensions.Logging;

namespace Inflow.Modules.Wallets.Application.Owners.Events.External.Handlers;

internal sealed class CustomerCompletedEventHandler : IEventHandler<CustomerCompleted>
{
    private readonly IIndividualOwnerRepository _ownerRepository;
    private readonly IClock _clock;
    private readonly ILogger<CustomerCompletedEventHandler> _logger;

    public CustomerCompletedEventHandler(IIndividualOwnerRepository ownerRepository, IClock clock, 
        ILogger<CustomerCompletedEventHandler> logger)
    {
        _ownerRepository = ownerRepository;
        _clock = clock;
        _logger = logger;
    }

    public async Task HandleAsync(CustomerCompleted @event, CancellationToken cancellationToken = default)
    {
        var owner = new IndividualOwner(@event.CustomerId, @event.Name, @event.FullName, _clock.CurrentDate());
        await _ownerRepository.AddAsync(owner);
        _logger.LogInformation($"Created individual owner with ID: '{owner.Id}' based on customer.");
    }
}