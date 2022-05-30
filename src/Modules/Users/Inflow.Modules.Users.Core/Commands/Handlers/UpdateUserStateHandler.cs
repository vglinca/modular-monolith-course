using System;
using System.Threading;
using System.Threading.Tasks;
using Inflow.Modules.Users.Core.Entities;
using Inflow.Modules.Users.Core.Events;
using Inflow.Modules.Users.Core.Exceptions;
using Inflow.Modules.Users.Core.Repositories;
using Inflow.Shared.Abstractions;
using Inflow.Shared.Abstractions.Commands;
using Inflow.Shared.Abstractions.Exceptions;
using Inflow.Shared.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Inflow.Modules.Users.Core.Commands.Handlers;

internal sealed class UpdateUserStateHandler : ICommandHandler<UpdateUserState>
{
    private readonly IUserRepository _repository;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<UpdateUserStateHandler> _logger;

    public UpdateUserStateHandler(IUserRepository repository, IMessageBroker messageBroker,
        ILogger<UpdateUserStateHandler> logger)
    {
        _repository = repository;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(UpdateUserState command, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<UserState>(command.State, true, out var state))
        {
            throw new InvalidUserStateException(command.State);
        }

        var user = await _repository.GetAsync(command.UserId)
            .IfNullThen(() => ResourceNotFoundException.OfType<User>(command.UserId));

        var previousState = user.State;
        if (previousState == state)
        {
            return;
        }

        if (user.RoleId == Role.Admin)
        {
            throw new UserStateCannotBeChangedException(command.State, command.UserId);
        }

        user.State = state;
        await _repository.UpdateAsync(user);
        await _messageBroker.PublishAsync(new UserStateUpdated(user.Id, state.ToString().ToLowerInvariant()),
            cancellationToken);

        _logger.LogInformation("Updated state for user with ID: '{UserId}', '{PreviousState}' -> '{NewState}'.",
            user.Id, previousState, state);
    }
}