using System.Threading;
using System.Threading.Tasks;
using Inflow.Shared.Abstractions.Messaging;

namespace Inflow.Shared.Infrastructure.Messaging;

internal sealed class AsyncMessageDispatcher : IAsyncMessageDispatcher
{
    private readonly IMessageChannel _messageChannel;
    public AsyncMessageDispatcher(IMessageChannel messageChannel) => _messageChannel = messageChannel;

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : class, IMessage
        => await _messageChannel.Writer.WriteAsync(new MessageEnvelope(message), cancellationToken);
}