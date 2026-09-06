namespace Chat.Application.Features.Messages.SendMessage;

public sealed class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.Body)
            .NotEmpty()
            .MaximumLength(2000);

        // An empty ClientMessageId would break idempotency, so it is rejected up front.
        RuleFor(x => x.ClientMessageId)
            .NotEqual(Guid.Empty);
    }
}
