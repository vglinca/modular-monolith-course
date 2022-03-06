using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Inflow.Shared.Abstractions.Dispatchers;
using Inflow.Shared.Abstractions.Time;
using Inflow.Shared.Infrastructure.Api;
using Inflow.Shared.Infrastructure.Commands;
using Inflow.Shared.Infrastructure.Dispatchers;
using Inflow.Shared.Infrastructure.Postgres;
using Inflow.Shared.Infrastructure.Queries;
using Inflow.Shared.Infrastructure.Time;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Inflow.Bootstrapper")]
namespace Inflow.Shared.Infrastructure;

internal static class Extensions
{
    public static IServiceCollection AddModularInfrastructure(this IServiceCollection services, 
        IList<Assembly> assemblies, IConfiguration configuration)
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
            .AddCommands(assemblies)
            .AddQueries(assemblies)
            .AddSingleton<IDispatcher, InMemoryDispatcher>()
            .AddPostgres()
            .AddSingleton<IClock, UtcClock>()
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
}