using System.Collections.Generic;
using Chronicle;
using Inflow.Shared.Abstractions.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Inflow.Modules.Saga.Api;

internal class SagaModule : IModule
{
    public string Name { get; } = "Saga";
    
    public IEnumerable<string> Policies { get; } = new[]
    {
        "saga"
    };
    
    public void Register(IServiceCollection services) => services.AddChronicle();

    public void Use(IApplicationBuilder app)
    {
    }
}