using System;
using System.Threading.Tasks;

namespace Inflow.Shared.Infrastructure.Messaging.Outbox.Interfaces;

public interface IInbox
{
    public bool Enabled { get; }
    Task HandleASync(Guid messageId, string name, Func<Task> handler);
    Task CleanupAsync(DateTime? to = null);
}