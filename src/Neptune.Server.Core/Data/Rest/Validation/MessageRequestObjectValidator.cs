using FluentValidation;

namespace Neptune.Server.Core.Data.Rest.Validation;

public class MessageRequestObjectValidator : AbstractValidator<MessageRequestObject>
{
    public MessageRequestObjectValidator()
    {
        RuleFor(x => x.To)
            .NotEmpty()
            .WithMessage("To field is required.");

        RuleFor(x => x.Message)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(200)
            .WithMessage("Message field is required.");
    }
}
