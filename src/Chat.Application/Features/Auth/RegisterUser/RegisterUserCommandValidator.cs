namespace Chat.Application.Features.Auth.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .Length(3, 32)
            .Matches("^[a-z0-9._-]+$")
            .WithMessage("Username may contain only lowercase letters, digits, dot, underscore or hyphen.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}