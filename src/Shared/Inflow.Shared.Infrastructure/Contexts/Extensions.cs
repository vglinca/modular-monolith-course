using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Inflow.Shared.Infrastructure.Contexts;

public static class Extensions
{
    public static IServiceCollection AddContext(this IServiceCollection services) =>
        services
            .AddSingleton<ContextAccessor>()
            .AddTransient(sp => sp.GetRequiredService<ContextAccessor>().Context);

    public static IApplicationBuilder UseContext(this IApplicationBuilder app)
    {
        app.Use((ctx, next) =>
        {
            ctx.RequestServices.GetRequiredService<ContextAccessor>().Context = new Context(ctx);

            return next();
        });

        return app;
    }
}