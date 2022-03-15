using System;
using System.Collections.Generic;
using System.Reflection;
using Inflow.Shared.Abstractions.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Inflow.Shared.Infrastructure.Queries;

internal static class Extensions
{
    public static IServiceCollection AddQueries(this IServiceCollection services, IList<Assembly> assemblies)
    {
        services.AddSingleton<IQueryDispatcher, InMemoryQueryDispatcher>();
        services.Scan(s =>
            s.FromAssemblies(assemblies).AddClasses(c =>
                    c.AssignableTo(typeof(IQueryHandler<,>))
                        .WithoutAttribute<DecoratorAttribute>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        
        return services;
    }
}