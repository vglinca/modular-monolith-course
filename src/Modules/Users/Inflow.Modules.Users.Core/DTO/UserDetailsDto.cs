using System.Collections.Generic;

namespace Inflow.Modules.Users.Core.DTO;

internal class UserDetailsDto : UserDto
{
    public IEnumerable<string> Permissions { get; set; }
}