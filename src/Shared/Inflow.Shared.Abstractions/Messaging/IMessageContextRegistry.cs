namespace Inflow.Shared.Abstractions.Messaging;

public interface IMessageContextRegistry
{
    void Set(IMessage message, IMessageContext context);
}