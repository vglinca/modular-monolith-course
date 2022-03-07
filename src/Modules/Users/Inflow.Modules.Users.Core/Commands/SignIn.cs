
using System;
using Inflow.Shared.Abstractions.Commands;

namespace Inflow.Modules.Users.Core.Commands;

internal record SignIn(string Email, string Password) : ICommand
{
    public Guid Id { get; init; } = Guid.NewGuid();
}