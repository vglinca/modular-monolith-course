using System.Threading.Channels;

namespace Inflow.Shared.Infrastructure.Messaging;

internal interface IMessageChannel
{
    ChannelReader<MessageEnvelope> Reader { get; }
    ChannelWriter<MessageEnvelope> Writer { get; }
}