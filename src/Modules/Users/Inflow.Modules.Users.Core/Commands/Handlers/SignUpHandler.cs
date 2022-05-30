using System.Linq;
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
using Inflow.Shared.Abstractions.Modules;
using Inflow.Shared.Abstractions.Time;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Inflow.Modules.Users.Core.Commands.Handlers;

internal sealed class SignUpHandler : ICommandHandler<SignUp>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IClock _clock;
    private readonly RegistrationOptions _registrationOptions;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<SignUpHandler> _logger;

    public SignUpHandler(IUserRepository userRepository, IRoleRepository roleRepository, 
        IPasswordHasher<User> passwordHasher, IClock clock, RegistrationOptions registrationOptions, 
        IMessageBroker messageBroker, ILogger<SignUpHandler> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _clock = clock;
        _registrationOptions = registrationOptions;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task HandleAsync(SignUp command, CancellationToken cancellationToken = default)
    {
        if (!_registrationOptions.Enabled)
        {
            throw new SignUpDisabledException();
        }

        var email = command.Email.ToLowerInvariant();
        var provider = email.Split("@").Last();
        
        if (_registrationOptions.InvalidEmailProviders?.Any(x => provider.Contains(x)) is true)
        {
            throw new InvalidEmailProviderException(email);
        }
        
        if (string.IsNullOrWhiteSpace(command.Password) || command.Password.Length is > 100 or < 6)
        {
            throw new InvalidPasswordException("not matching the criteria");
        }

        var user = await _userRepository.GetAsync(email);
        if (user is not null)
        {
            throw new EmailAlreadyInUseException();
        }
        
        var roleName = string.IsNullOrWhiteSpace(command.Role) ? Role.Default : command.Role.ToLowerInvariant();
        var role = await _roleRepository.GetAsync(roleName)
            .IfNullThen(() => new ResourceNotFoundException($"Role '{roleName}' was not found."));
        
        var now = _clock.CurrentDate();
        var password = _passwordHasher.HashPassword(default, command.Password);
        user = new User
        {
            Id = command.UserId,
            Email = email,
            Password = password,
            Role = role,
            CreatedAt = now,
            State = UserState.Active,
        };
        
        await _userRepository.AddAsync(user);
        await _messageBroker.PublishAsync(new UserSignedUp(user.Id, user.Email, user.Role.Name), cancellationToken);
        
        _logger.LogInformation($"User with ID: '{user.Id}' has signed up.");
    }
}