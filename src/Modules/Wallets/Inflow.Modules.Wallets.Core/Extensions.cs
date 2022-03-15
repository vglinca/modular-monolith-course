using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Inflow.Modules.Wallets.Api")]
[assembly: InternalsVisibleTo("Inflow.Modules.Wallets.Application")]
[assembly: InternalsVisibleTo("Inflow.Modules.Wallets.Infrastructure")]
namespace Inflow.Modules.Wallets.Core;

internal static class Extensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        return services;
    }
}