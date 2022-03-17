using System.Threading;
using System.Threading.Tasks;
using Inflow.Shared.Abstractions.Commands;

namespace Inflow.Modules.Users.Core.Commands.Handlers;

internal sealed class SignOutHandler : ICommandHandler<SignOut>
{
    public async Task HandleAsync(SignOut command, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
    }
}