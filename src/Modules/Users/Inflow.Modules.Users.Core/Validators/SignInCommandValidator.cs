using FluentValidation;
using Inflow.Modules.Users.Core.Commands;

namespace Inflow.Modules.Users.Core.Validators;

internal class SignInCommandValidator : AbstractValidator<SignIn>
{
    public SignInCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty()
            .Matches(
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$");
        RuleFor(x => x.Password).NotEmpty();
    }
}