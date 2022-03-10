using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Inflow.Shared.Abstractions.Dispatchers;
using Inflow.Shared.Abstractions.Modules;
using Inflow.Shared.Abstractions.Storage;
using Inflow.Shared.Abstractions.Time;
using Inflow.Shared.Infrastructure.Api;
using Inflow.Shared.Infrastructure.Auth;
using Inflow.Shared.Infrastructure.Commands;
using Inflow.Shared.Infrastructure.Contracts;
using Inflow.Shared.Infrastructure.Dispatchers;
using Inflow.Shared.Infrastructure.Events;
using Inflow.Shared.Infrastructure.Messaging;
using Inflow.Shared.Infrastructure.Modules;
using Inflow.Shared.Infrastructure.Postgres;
using Inflow.Shared.Infrastructure.Queries;
using Inflow.Shared.Infrastructure.Services;
using Inflow.Shared.Infrastructure.Storage;
using Inflow.Shared.Infrastructure.Time;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inflow.Shared.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddModularInfrastructure(this IServiceCollection services, 
        IList<Assembly> assemblies, IConfiguration configuration, IList<IModule> modules)
    {
        var disabledModules = new List<string>();
        foreach (var (key, value) in configuration.AsEnumerable())
        {
            if (!key.Contains(":module:enabled"))
            {
                continue;
            }

            if (!bool.Parse(value))
            {
                disabledModules.Add(key.Split(":")[0]);
            }
        }
        
        services
            .AddMemoryCache()
            .AddSingleton<IRequestStorage, RequestStorage>()
            .AddCommands(assemblies)
            .AddQueries(assemblies)
            .AddEvents(assemblies)
            .AddAuth(modules)
            .AddSingleton<IDispatcher, InMemoryDispatcher>()
            .AddPostgres()
            .AddSingleton<IClock, UtcClock>()
            .AddHostedService<DbContextAppInitializer>()
            .AddModuleRequests(assemblies)
            .AddMessaging()
            .AddContracts()
            .AddControllers()
            .ConfigureApplicationPartManager(mgr =>
            {
                var removedParts = new List<ApplicationPart>();
                foreach (var disabledModule in disabledModules)
                {
                    var parts = mgr.ApplicationParts
                        .Where(x => x.Name.Contains(disabledModule, StringComparison.InvariantCultureIgnoreCase));
                    removedParts.AddRange(parts);
                }

                foreach (var removedPart in removedParts)
                {
                    mgr.ApplicationParts.Remove(removedPart);
                }
                
                mgr.FeatureProviders.Add(new InternalControllerFeatureProvider());
            });

        return services;
    }

    public static IServiceCollection AddInitializer<T>(this IServiceCollection services) where T : class, IInitializer
        => services.AddTransient<IInitializer, T>();

    public static string GetModuleName(this object obj)
        => obj?.GetType().GetModuleName() ?? string.Empty;

    public static string GetModuleName(this Type type, string namespacePart = "Modules", int splitIndex = 2)
    {
        if (type?.Namespace is null)
        {
            return string.Empty;
        }

        return type.Namespace.Contains(namespacePart)
            ? type.Namespace.Split(".")[splitIndex].ToLowerInvariant()
            : string.Empty;
    }

    public static TOptions GetOptions<TOptions>(this IServiceCollection services, string section)
        where TOptions : class, new()
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        var configuration = scope.ServiceProvider.GetService<IConfiguration>();

        return configuration.GetOptions<TOptions>(section);
    }

    public static TOptions GetOptions<TOptions>(this IConfiguration configuration, string section) 
        where TOptions : class, new()
    {
        var options = new TOptions();
        configuration.GetSection(section).Bind(options);

        return options;
    }

    public static IApplicationBuilder UseModularInfrastructure(this IApplicationBuilder app)
    {
        app.UseForwardedHeaders(new ForwardedHeadersOptions() {ForwardedHeaders = ForwardedHeaders.All});
        
        app.UseAuth();
        app.UseRouting();
        app.UseAuthorization();

        return app;
    }
}