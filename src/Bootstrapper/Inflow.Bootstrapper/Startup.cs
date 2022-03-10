using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Inflow.Shared.Abstractions.Modules;
using Inflow.Shared.Infrastructure;
using Inflow.Shared.Infrastructure.Contracts;
using Inflow.Shared.Infrastructure.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Inflow.Bootstrapper;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IList<Assembly> _assemblies;
    private readonly IList<IModule> _modules;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
        _assemblies = ModuleLoader.LoadAssemblies(_configuration);
        _modules = ModuleLoader.LoadModules(_assemblies);
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddModularInfrastructure(_assemblies, _configuration, _modules);

        foreach (var module in _modules)
        {
            module.Register(services);
        }
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        logger.LogInformation($"Loading modules: {string.Join(",", _modules.Select(m => m.Name))}");
        
        app.UseModularInfrastructure();

        foreach (var module in _modules)
        {
            module.Use(app);
        }

        app.ValidateContracts(_assemblies);
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", context => context.Response.WriteAsync("Inflow API"));
        });

        _assemblies.Clear();
        _modules.Clear();
    }
}