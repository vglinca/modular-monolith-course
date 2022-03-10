using System.Runtime.CompilerServices;
using Inflow.Modules.Customers.Core;
using Inflow.Modules.Customers.Core.Events.External;
using Inflow.Shared.Abstractions.Modules;
using Inflow.Shared.Infrastructure.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Inflow.Bootstrapper")]
namespace Inflow.Modules.Customers.Api;

internal class CustomersModule : IModule
{
    public string Name { get; } = "Customers";
    
    public void Register(IServiceCollection services)
    {
        services.AddCore();
    }

    public void Use(IApplicationBuilder app)
    {
        app.UseContracts()
            .Register<UserSignedUpContract>();
    }
}