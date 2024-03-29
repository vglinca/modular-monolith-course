using System.Collections.Generic;
using Inflow.Modules.Wallets.Application;
using Inflow.Modules.Wallets.Application.Owners.Events.External;
using Inflow.Modules.Wallets.Core;
using Inflow.Modules.Wallets.Infrastructure;
using Inflow.Shared.Abstractions.Modules;
using Inflow.Shared.Infrastructure.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Inflow.Modules.Wallets.Api;

internal class WalletsModule : IModule
{
    public string Name { get; } = "Wallets";

    public IEnumerable<string> Policies { get; } = new[]
    {
        "transfers", "wallets"
    };
    public void Register(IServiceCollection services)
    {
        services
            .AddCore()
            .AddApplication()
            .AddInfrastructure();
    }

    public void Use(IApplicationBuilder app)
    {
        app.UseContracts()
            .Register<CustomerCompletedContract>()
            .Register<CustomerVerifiedContract>();
    }
}