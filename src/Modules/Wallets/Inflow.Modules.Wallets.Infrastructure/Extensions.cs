using System.Runtime.CompilerServices;
using Inflow.Modules.Wallets.Application.Wallets.Storage;
using Inflow.Modules.Wallets.Core.Owners.Repositories;
using Inflow.Modules.Wallets.Core.Wallets.Repositories;
using Inflow.Modules.Wallets.Infrastructure.EF;
using Inflow.Modules.Wallets.Infrastructure.EF.Repositories;
using Inflow.Modules.Wallets.Infrastructure.Storage;
using Inflow.Shared.Infrastructure.Messaging.Outbox;
using Inflow.Shared.Infrastructure.Postgres;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Inflow.Modules.Wallets.Api")]
namespace Inflow.Modules.Wallets.Infrastructure;

internal static class Extensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        return services
            .AddScoped<ICorporateOwnerRepository, CorporateOwnerRepository>()
            .AddScoped<IIndividualOwnerRepository, IndividualOwnerRepository>()
            .AddScoped<IWalletRepository, WalletRepository>()
            .AddScoped<ITransferStorage, TransferStorage>()
            .AddScoped<IWalletStorage, WalletStorage>()
            .AddPostgres<WalletsDbContext>()
            .AddOutbox<WalletsDbContext>()
            .AddUnitOfWork<WalletsUnitOfWork>();
    }
}