using System;
using Inflow.Shared.Abstractions.Commands;

namespace Inflow.Modules.Users.Core.Commands;

internal record SignUp(string Email, string Password, string Role) : ICommand
{
    public Guid UserId { get; init; } = Guid.NewGuid();
}