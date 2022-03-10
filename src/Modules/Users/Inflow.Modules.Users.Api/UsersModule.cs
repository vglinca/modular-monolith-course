using System.Collections.Generic;
using Inflow.Modules.Users.Core;
using Inflow.Modules.Users.Core.DTO;
using Inflow.Modules.Users.Core.Queries;
using Inflow.Shared.Abstractions.Dispatchers;
using Inflow.Shared.Abstractions.Modules;
using Inflow.Shared.Infrastructure.Modules;
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

    public void Register(IServiceCollection services) => services.AddCore();

    public void Use(IApplicationBuilder app)
    {
        app.UseModuleRequests()
            .Subscribe<GetUserByEmail, UserDetailsDto>("users/get-by-email",
                (query, sp, token) =>
                    sp.GetRequiredService<IDispatcher>().QueryAsync(query, token));
    }
}