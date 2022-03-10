using System.Collections.Generic;

namespace Inflow.Shared.Infrastructure.Modules;

internal interface IModuleRegistry
{
    ModuleRequestRegistration GetRequestRegistration(string path);
    IEnumerable<ModuleBroadcastRegistration> GetBroadcastRegistrations(string key);
    void AddRequestAction(string path, ModuleRequestRegistration requestRegistration);
    void AddBroadcastAction(ModuleBroadcastRegistration broadcastRegistration);
}