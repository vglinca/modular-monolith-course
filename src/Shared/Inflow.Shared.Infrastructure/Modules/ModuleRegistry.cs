using System;
using System.Collections.Generic;
using System.Linq;

namespace Inflow.Shared.Infrastructure.Modules;

internal sealed class ModuleRegistry : IModuleRegistry
{
    private readonly Dictionary<string, ModuleRequestRegistration> _requestRegistrations = new();
    private readonly List<ModuleBroadcastRegistration> _broadcastRegistrations = new();

    public ModuleRequestRegistration GetRequestRegistration(string path)
        => _requestRegistrations.TryGetValue(path, out var registration) ? registration : null;

    public IEnumerable<ModuleBroadcastRegistration> GetBroadcastRegistrations(string key)
        => _broadcastRegistrations.Where(x => x.Key.Equals(key));

    public void AddRequestAction(string path, ModuleRequestRegistration requestRegistration)
    {
        if (path is null)
        {
            throw new InvalidOperationException("Request path cannot be null");
        }

        if (requestRegistration.GetType().Namespace is null) 
        {
            throw new InvalidOperationException("Request namespace cannot be null");
        }
        
        _requestRegistrations.Add(path, requestRegistration);
    }

    public void AddBroadcastAction(ModuleBroadcastRegistration broadcastRegistration)
    {
        if (_broadcastRegistrations.GetType().Namespace is null) 
        {
            throw new InvalidOperationException("Receiver namespace cannot be null");
        }
        
        _broadcastRegistrations.Add(broadcastRegistration);
    }
}