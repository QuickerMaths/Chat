namespace Chat.Api.Endpoints;

public sealed record SendMessageRequest(string Body, Guid ClientMessageId);
