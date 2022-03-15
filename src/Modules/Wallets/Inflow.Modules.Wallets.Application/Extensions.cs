using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Inflow.Modules.Wallets.Api")]
[assembly: InternalsVisibleTo("Inflow.Modules.Wallets.Infrastructure")]
namespace Inflow.Modules.Wallets.Application;

internal static class Extensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}