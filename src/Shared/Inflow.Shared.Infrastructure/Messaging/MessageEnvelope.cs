using Inflow.Shared.Abstractions.Messaging;

namespace Inflow.Shared.Infrastructure.Messaging;

internal record MessageEnvelope(IMessage Message);