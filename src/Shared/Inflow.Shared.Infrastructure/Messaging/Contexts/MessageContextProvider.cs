using Inflow.Shared.Abstractions.Messaging;
using Microsoft.Extensions.Caching.Memory;

namespace Inflow.Shared.Infrastructure.Messaging.Contexts;

public class MessageContextProvider : IMessageContextProvider
{
    private readonly IMemoryCache _cache;

    public MessageContextProvider(IMemoryCache cache) => _cache = cache;
    
    public IMessageContext Get(IMessage message) => _cache.Get<IMessageContext>(message);
}