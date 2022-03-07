using System.Runtime.CompilerServices;
using FluentValidation;
using FluentValidation.AspNetCore;
using Inflow.Modules.Users.Core.DAL;
using Inflow.Modules.Users.Core.DAL.Repositories;
using Inflow.Modules.Users.Core.Entities;
using Inflow.Modules.Users.Core.Repositories;
using Inflow.Modules.Users.Core.Services;
using Inflow.Shared.Infrastructure;
using Inflow.Shared.Infrastructure.Postgres;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Inflow.Modules.Users.Api")]
namespace Inflow.Modules.Users.Core;

internal static class Extensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        var registrationOptions = services.GetOptions<RegistrationOptions>("users:registration");
        services.AddSingleton(registrationOptions);

        return services
            .AddFluentValidation(x => x.RegisterValidatorsFromAssembly(typeof(UsersDbContext).Assembly))
            .AddSingleton<IUserRequestStorage, UserRequestStorage>()
            .AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>()
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IRoleRepository, RoleRepository>()
            .AddPostgres<UsersDbContext>()
            .AddInitializer<UsersInitializer>();
    }
}