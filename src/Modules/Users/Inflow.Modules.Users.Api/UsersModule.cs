using System.Collections.Generic;
using Inflow.Modules.Users.Core;
using Inflow.Shared.Abstractions.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Inflow.Modules.Users.Api;

public class UsersModule : IModule
{
    public string Name { get; } = "Users";
    public IEnumerable<string> Policies { get; } = new[]
    {
        "users"
    };

    public void Register(IServiceCollection services)
    {
        services.AddCore();
    }

    public void Use(IApplicationBuilder app)
    {
        
    }
}