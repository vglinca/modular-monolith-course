using System.Threading;
using System.Threading.Tasks;
using Inflow.Shared.Abstractions.Messaging;

namespace Inflow.Shared.Infrastructure.Messaging;

internal interface IAsyncMessageDispatcher
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class, IMessage;
}