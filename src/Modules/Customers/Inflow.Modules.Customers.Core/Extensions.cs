using System.Runtime.CompilerServices;
using Inflow.Modules.Customers.Core.Clients;
using Inflow.Modules.Customers.Core.DAL;
using Inflow.Modules.Customers.Core.DAL.Repositories;
using Inflow.Modules.Customers.Core.Domain.Repositories;
using Inflow.Shared.Infrastructure.Messaging.Outbox;
using Inflow.Shared.Infrastructure.Postgres;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Inflow.Modules.Customers.Api")]
namespace Inflow.Modules.Customers.Core;

internal static class Extensions
{
    public static IServiceCollection AddCore(this IServiceCollection services) =>
        services
            .AddSingleton<IUserApiClient, UserApiClient>()
            .AddPostgres<CustomersDbContext>()
            .AddScoped<ICustomerRepository, CustomersRepository>()
            .AddOutbox<CustomersDbContext>()
            .AddUnitOfWork<CustomersUnitOfWork>();
}