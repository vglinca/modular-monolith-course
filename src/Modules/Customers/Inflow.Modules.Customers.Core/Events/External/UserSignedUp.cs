using System;
using Inflow.Shared.Abstractions.Contracts;
using Inflow.Shared.Abstractions.Events;
using Inflow.Shared.Abstractions.Messaging;

namespace Inflow.Modules.Customers.Core.Events.External;

internal record UserSignedUp(Guid UserId, string Email, string Role) : IEvent;

[Message("users")]
internal class UserSignedUpContract : Contract<UserSignedUp>
{
    public UserSignedUpContract()
    {
        RequireAll();
    }
}