using System;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Shared.Abstractions.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace Inflow.Shared.Infrastructure.Modules;

internal sealed class ModuleSubscriber : IModuleSubscriber
{
    private readonly IModuleRegistry _registry;
    private readonly IServiceProvider _serviceProvider;

    public ModuleSubscriber(IModuleRegistry registry, IServiceProvider serviceProvider)
    {
        _registry = registry;
        _serviceProvider = serviceProvider;
    }
    
    public IModuleSubscriber Subscribe<TRequest, TResponse>(string path, 
        Func<TRequest, IServiceProvider, CancellationToken, Task<TResponse>> callback) 
        where TRequest : class where TResponse : class
    {
        var registration = new ModuleRequestRegistration(typeof(TRequest), typeof(TResponse),
            async (req, token) =>
            {
                using var scope = _serviceProvider.CreateScope();
                return await callback((TRequest) req, scope.ServiceProvider, token);
            });
        
        _registry.AddRequestAction(path, registration);
        
        return this;
    }
}