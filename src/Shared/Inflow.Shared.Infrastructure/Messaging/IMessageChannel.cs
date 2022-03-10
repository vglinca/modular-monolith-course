using System.Threading.Channels;
using Inflow.Shared.Abstractions.Messaging;

namespace Inflow.Shared.Infrastructure.Messaging;

internal interface IMessageChannel
{
    ChannelReader<MessageEnvelope> Reader { get; }
    ChannelWriter<MessageEnvelope> Writer { get; }
}