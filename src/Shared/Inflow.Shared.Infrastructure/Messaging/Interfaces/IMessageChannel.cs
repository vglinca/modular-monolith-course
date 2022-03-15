using System.Threading.Channels;
using Inflow.Shared.Infrastructure.Messaging.Dispatching;

namespace Inflow.Shared.Infrastructure.Messaging.Interfaces;

internal interface IMessageChannel
{
    ChannelReader<MessageEnvelope> Reader { get; }
    ChannelWriter<MessageEnvelope> Writer { get; }
}