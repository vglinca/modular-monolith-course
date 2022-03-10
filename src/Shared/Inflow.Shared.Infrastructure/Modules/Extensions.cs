using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Inflow.Shared.Abstractions.Commands;
using Inflow.Shared.Abstractions.Events;
using Inflow.Shared.Abstractions.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Inflow.Shared.Infrastructure.Modules;

public static class Extensions
{
    public static IHostBuilder ConfigureModules(this IHostBuilder builder)
        => builder.ConfigureAppConfiguration((ctx, cfg) =>
        {
            foreach (var setting in GetSettings("*"))
            {
                cfg.AddJsonFile(setting);
            }

            IEnumerable<string> GetSettings(string pattern)
                => Directory.EnumerateFiles(ctx.HostingEnvironment.ContentRootPath, $"module.{pattern}.json",
                    SearchOption.AllDirectories);
        });

    internal static IServiceCollection AddModuleRequests(this IServiceCollection services, IEnumerable<Assembly> assemblies) =>
        services
            .AddModuleRegistry(assemblies)
            .AddSingleton<IModuleSubscriber, ModuleSubscriber>()
            .AddSingleton<IModuleClient, ModuleClient>()
            .AddSingleton<IModuleSerializer, JsonModuleSerializer>();

    private static IServiceCollection AddModuleRegistry(this IServiceCollection services,
        IEnumerable<Assembly> assemblies)
    {
        var registry = new ModuleRegistry();
        var types = assemblies.SelectMany(x => x.GetTypes()).ToArray();

        var eventTypes = types
            .Where(x => x.IsClass && typeof(IEvent).IsAssignableFrom(x))
            .ToArray();
        var commandTypes = types
            .Where(x => x.IsClass && typeof(ICommand).IsAssignableFrom(x))
            .ToArray();

        services.AddSingleton<IModuleRegistry>(sp =>
        {
            var commandDispatcher = sp.GetRequiredService<ICommandDispatcher>();
            var eventDispatcher = sp.GetRequiredService<IEventDispatcher>();

            foreach (var eventType in eventTypes)
            {
                var registration = new ModuleBroadcastRegistration(eventType, (@event, token) =>
                    (Task) eventDispatcher.GetType().GetMethod(nameof(eventDispatcher.PublishAsync))
                        ?.MakeGenericMethod(eventType)
                        .Invoke(eventDispatcher, new[] {@event, token}));
                registry.AddBroadcastAction(registration);
            }

            foreach (var commandType in commandTypes)
            {
                var registration = new ModuleBroadcastRegistration(commandType, (@event, token) =>
                    (Task) commandDispatcher.GetType().GetMethod(nameof(commandDispatcher.DispatchAsync))
                        ?.MakeGenericMethod(commandType)
                        .Invoke(commandDispatcher, new[] {@event, token}));
                registry.AddBroadcastAction(registration);
            }

            return registry;
        });

        return services;
    }

    public static IModuleSubscriber UseModuleRequests(this IApplicationBuilder app)
        => app.ApplicationServices.GetService<IModuleSubscriber>();
}