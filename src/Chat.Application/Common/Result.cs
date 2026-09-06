namespace Chat.Application.Common;

public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Forbidden
}

#pragma warning disable CA1000
public sealed record ApplicationError(ErrorType Type, string Code, string Message)
{
    public static ApplicationError NotFound(string code, string message) => new ApplicationError(ErrorType.NotFound, code, message);
    public static ApplicationError Conflict(string code, string message) => new ApplicationError(ErrorType.Conflict, code, message);
    public static ApplicationError Forbidden(string code, string message) => new ApplicationError(ErrorType.Forbidden, code, message);
    public static ApplicationError Validation(string code, string message) => new ApplicationError(ErrorType.Validation, code, message);
}

public readonly record struct Result<T>
{
    public bool IsSuccess { get; private init; }
    public T? Value { get; private init; }
    public ApplicationError? Error { get; private init; }
    
    public static Result<T> Success(T value) => new(){IsSuccess =  true, Value =  value};
    public static Result<T> Failure(ApplicationError error) => new() {IsSuccess = false, Error = error};
}