using Inflow.Shared.Abstractions.Messaging;
using Inflow.Shared.Infrastructure.Messaging.Brokers;
using Inflow.Shared.Infrastructure.Messaging.Contexts;
using Inflow.Shared.Infrastructure.Messaging.Dispatching;
using Inflow.Shared.Infrastructure.Messaging.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Inflow.Shared.Infrastructure.Messaging;

internal static class Extensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        var messagingOptions = services.GetOptions<MessagingOptions>("messaging");
        services
            .AddTransient<IMessageBroker, InMemoryMessageBroker>()
            .AddSingleton<IMessageChannel, MessageChannel>()
            .AddSingleton<IAsyncMessageDispatcher, AsyncMessageDispatcher>()
            .AddSingleton<IMessageContextProvider, MessageContextProvider>()
            .AddSingleton<IMessageContextRegistry, MessageContextRegistry>()
            .AddSingleton(messagingOptions);

        if (messagingOptions.UseASyncDispatcher)
        {
            services.AddHostedService<AsyncDispatcherJob>();
        }
        
        return services;
    }
}