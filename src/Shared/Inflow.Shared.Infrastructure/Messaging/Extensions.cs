using Inflow.Shared.Abstractions.Messaging;
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
            .AddSingleton(messagingOptions);

        if (messagingOptions.UseASyncDispatcher)
        {
            services.AddHostedService<AsyncDispatcherJob>();
        }
        
        return services;
    }
}