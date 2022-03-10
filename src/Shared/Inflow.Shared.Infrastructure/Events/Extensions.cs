using System;
using System.Collections.Generic;
using System.Reflection;
using Inflow.Shared.Abstractions.Commands;
using Inflow.Shared.Abstractions.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Inflow.Shared.Infrastructure.Events;

internal static class Extensions
{
    public static IServiceCollection AddEvents(this IServiceCollection services, IList<Assembly> assemblies)
    {
        services.AddSingleton<IEventDispatcher, EventDispatcher>();
        services.Scan(s =>
            s.FromAssemblies(assemblies).AddClasses(c =>
                    c.AssignableTo(typeof(IEventHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        
        return services;
    }
}