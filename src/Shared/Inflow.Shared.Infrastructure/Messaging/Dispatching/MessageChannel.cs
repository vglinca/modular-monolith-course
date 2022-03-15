using System.Threading.Channels;
using Inflow.Shared.Infrastructure.Messaging.Interfaces;

namespace Inflow.Shared.Infrastructure.Messaging.Dispatching;

internal sealed class MessageChannel : IMessageChannel
{
    private readonly Channel<MessageEnvelope> _messages = Channel.CreateUnbounded<MessageEnvelope>();

    public ChannelReader<MessageEnvelope> Reader => _messages.Reader;
    public ChannelWriter<MessageEnvelope> Writer => _messages.Writer;
}