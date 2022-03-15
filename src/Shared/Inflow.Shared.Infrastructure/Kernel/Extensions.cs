using System.Collections.Generic;
using System.Reflection;
using Inflow.Shared.Abstractions.Kernel;
using Microsoft.Extensions.DependencyInjection;

namespace Inflow.Shared.Infrastructure.Kernel;

internal static class Extensions
{
    public static IServiceCollection
        AddDomainEvents(this IServiceCollection services, IEnumerable<Assembly> assemblies) =>
        services
            .AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>()
            .Scan(x => x.FromAssemblies(assemblies)
                .AddClasses(y => y.AssignableTo(typeof(IDomainEventHandler<>))).AsImplementedInterfaces()
                .WithScopedLifetime());
}