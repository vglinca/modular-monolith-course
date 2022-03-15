using Inflow.Shared.Abstractions.Messaging;

namespace Inflow.Shared.Infrastructure.Messaging.Dispatching;

internal record MessageEnvelope(IMessage Message, IMessageContext MessageContext);