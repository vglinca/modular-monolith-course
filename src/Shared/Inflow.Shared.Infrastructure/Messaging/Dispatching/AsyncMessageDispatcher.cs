using System.Threading;
using System.Threading.Tasks;
using Inflow.Shared.Abstractions.Messaging;
using Inflow.Shared.Infrastructure.Messaging.Interfaces;

namespace Inflow.Shared.Infrastructure.Messaging.Dispatching;

internal sealed class AsyncMessageDispatcher : IAsyncMessageDispatcher
{
    private readonly IMessageChannel _messageChannel;
    private readonly IMessageContextProvider _contextProvider;
    public AsyncMessageDispatcher(IMessageChannel messageChannel, IMessageContextProvider contextProvider)
    {
        _messageChannel = messageChannel;
        _contextProvider = contextProvider;
    }

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : class, IMessage
    {
        var messageContext = _contextProvider.Get(message);
        await _messageChannel.Writer.WriteAsync(new MessageEnvelope(message, messageContext), cancellationToken);
    }
}