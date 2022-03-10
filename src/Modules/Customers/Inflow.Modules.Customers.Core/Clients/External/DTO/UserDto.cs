using System;

namespace Inflow.Modules.Customers.Core.Clients.External.DTO;

internal class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}