using System.Threading;
using System.Threading.Tasks;
using Chronicle;
using Inflow.Modules.Saga.Api.Messages;
using Inflow.Shared.Abstractions.Events;

namespace Inflow.Modules.Saga.Api.Handlers;

internal sealed class SagaEventsHandler :
    IEventHandler<CustomerVerified>,
    IEventHandler<DepositCompleted>,
    IEventHandler<FundsAdded>,
    IEventHandler<WalletAdded>
{
    private readonly ISagaCoordinator _coordinator;

    public SagaEventsHandler(ISagaCoordinator coordinator) => _coordinator = coordinator;

    public Task HandleAsync(CustomerVerified @event, CancellationToken cancellationToken = default)
        => ProcessAsync(@event);

    public Task HandleAsync(DepositCompleted @event, CancellationToken cancellationToken = default)
        => ProcessAsync(@event);

    public Task HandleAsync(FundsAdded @event, CancellationToken cancellationToken = default)
        => ProcessAsync(@event);

    public Task HandleAsync(WalletAdded @event, CancellationToken cancellationToken = default)
        => ProcessAsync(@event);

    private Task ProcessAsync<T>(T message) where T : class => _coordinator.ProcessAsync(message, SagaContext.Empty);
}