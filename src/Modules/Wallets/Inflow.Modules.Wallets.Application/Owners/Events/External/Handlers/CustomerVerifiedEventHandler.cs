using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Wallets.Core.Owners.Exceptions;
using Inflow.Modules.Wallets.Core.Owners.Repositories;
using Inflow.Shared.Abstractions.Events;
using Inflow.Shared.Abstractions.Time;
using Microsoft.Extensions.Logging;

namespace Inflow.Modules.Wallets.Application.Owners.Events.External.Handlers;

internal sealed class CustomerVerifiedEventHandler : IEventHandler<CustomerVerified>
{
    private readonly IIndividualOwnerRepository _ownerRepository;
    private readonly IClock _clock;
    private readonly ILogger<CustomerVerifiedEventHandler> _logger;

    public CustomerVerifiedEventHandler(IIndividualOwnerRepository ownerRepository, IClock clock, 
        ILogger<CustomerVerifiedEventHandler> logger)
    {
        _ownerRepository = ownerRepository;
        _clock = clock;
        _logger = logger;
    }

    public async Task HandleAsync(CustomerVerified @event, CancellationToken cancellationToken = default)
    {
        var owner = await _ownerRepository.GetAsync(@event.CustomerId);
        if (owner is null)
        {
            throw new OwnerNotFoundException(@event.CustomerId);
        }
        
        owner.Verify(_clock.CurrentDate());
        await _ownerRepository.UpdateAsync(owner);
        
        _logger.LogInformation("Verified individual owner with ID: '{OwnerId}' based on customer.", owner.Id);
    }
}