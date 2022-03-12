using System.Runtime.CompilerServices;
using Inflow.Modules.Customers.Core;
using Inflow.Modules.Customers.Core.DTO;
using Inflow.Modules.Customers.Core.Events.External;
using Inflow.Modules.Customers.Core.Queries;
using Inflow.Shared.Abstractions.Modules;
using Inflow.Shared.Abstractions.Queries;
using Inflow.Shared.Infrastructure.Contracts;
using Inflow.Shared.Infrastructure.Modules;
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
        app.UseModuleRequests()
            .Subscribe<GetCustomer, CustomerDetailsDto>("customers/get",
                (query, sp, token) => 
                    sp.GetRequiredService<IQueryDispatcher>().DispatchAsync(query, token));
        app.UseContracts()
            .Register<UserSignedUpContract>();
    }
}