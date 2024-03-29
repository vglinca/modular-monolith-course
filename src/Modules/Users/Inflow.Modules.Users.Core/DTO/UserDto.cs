using System;

namespace Inflow.Modules.Users.Core.DTO;

internal class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public string State { get; set; }
    public DateTime CreatedAt { get; set; }
}