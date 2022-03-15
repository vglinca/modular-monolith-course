using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;

namespace Inflow.Shared.Abstractions.Kernel.Types;

public abstract class AggregateRoot<T>
{
    public T Id { get; protected set; }
    public int Version { get; protected set; } = 1;
    public IEnumerable<IDomainEvent> DomainEvents => _events;

    private readonly IList<IDomainEvent> _events = new List<IDomainEvent>();
    private bool _versionIncremented;

    protected void AddDomainEvent(IDomainEvent @event)
    {
        if (!_events.Any() && !_versionIncremented)
        {
            Version++;
            _versionIncremented = true;
        }
        
        _events.Add(@event);
    }

    public void ClearDomainEvents() => _events.Clear();

    protected void IncrementVersion()
    {
        if (_versionIncremented)
        {
            return;
        }

        Version++;
        _versionIncremented = true;
    }
}

public abstract class AggregateRoot : AggregateRoot<AggregateId> { }