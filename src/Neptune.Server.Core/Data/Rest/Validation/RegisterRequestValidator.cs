using FluentValidation;

namespace Neptune.Server.Core.Data.Rest.Validation;

public class RegisterRequestValidator : AbstractValidator<RegisterRequestObject>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required.")
            .Length(3, 20)
            .WithMessage("Username must be between 3 and 20 characters.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .Length(6, 100)
            .WithMessage("Password must be between 6 and 100 characters.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Invalid email format.");
    }
}
