using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Inflow.Shared.Infrastructure.Contracts;

public static class Extensions
{
    public static IServiceCollection AddContracts(this IServiceCollection services)
        => services.AddSingleton<IContractRegistry, ContractRegistry>();

    public static IApplicationBuilder ValidateContracts(this IApplicationBuilder app, IEnumerable<Assembly> assemblies)
    {
        var contractRegistry = app.ApplicationServices.GetService<IContractRegistry>();
        contractRegistry!.Validate(assemblies);

        return app;
    }

    public static IContractRegistry UseContracts(this IApplicationBuilder app)
        => app.ApplicationServices.GetService<IContractRegistry>();
}