using Inflow.Shared.Abstractions.Events;
using Inflow.Shared.Infrastructure.Messaging.Outbox.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Inflow.Shared.Infrastructure.Messaging.Outbox;

public static class Extensions
{
    public static IServiceCollection AddOutbox<T>(this IServiceCollection services) where T : DbContext
    {
        var outboxOptions = services.GetOptions<OutboxOptions>("outbox");
        if (!outboxOptions.Enabled)
        {
            return services;
        }

        services
            .AddTransient<IInbox, EfInbox<T>>()
            .AddTransient<IOutbox, EfOutbox<T>>()
            .AddTransient<EfInbox<T>>()
            .AddTransient<EfOutbox<T>>();

        using var sp = services.BuildServiceProvider();
        sp.GetRequiredService<InboxTypeRegistry>().Register<EfInbox<T>>();
        sp.GetRequiredService<OutboxTypeRegistry>().Register<EfOutbox<T>>();

        return services;
    }

    internal static IServiceCollection AddOutbox(this IServiceCollection services)
    {
        var outboxOptions = services.GetOptions<OutboxOptions>("outbox");
        services
            .AddSingleton(outboxOptions)
            .AddSingleton(new InboxTypeRegistry())
            .AddSingleton(new OutboxTypeRegistry())
            .AddSingleton<IOutboxBroker, OutboxBroker>();

        if (!outboxOptions.Enabled)
        {
            return services;
        }

        services.TryDecorate(typeof(IEventHandler<>), typeof(InboxEventHandlerDecorator<>));
        services
            .AddHostedService<OutboxProcessor>()
            .AddHostedService<OutboxCleanupProcessor>()
            .AddHostedService<InboxCleanupProcessor>();

        return services;
    }
}