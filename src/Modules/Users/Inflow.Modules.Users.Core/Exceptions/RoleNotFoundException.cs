using Inflow.Shared.Abstractions.Exceptions;

namespace Inflow.Modules.Users.Core.Exceptions;

internal class RoleNotFoundException : ResourceNotFoundException
{
    public string Role { get; }

    public RoleNotFoundException(string role) : base($"Role: '{role}' was not found.")
    {
        Role = role;
    }
}