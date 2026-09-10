using FluentValidation;

namespace Chat.Api.Endpoints;

public sealed class ValidationFilter<T> : IEndpointFilter where T : notnull
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
        if(validator is null)
            return await next(context);

        var argument = context.Arguments.OfType<T>().FirstOrDefault();
        if (argument is null)
            return await next(context);

        var validation = await validator.ValidateAsync(argument);
        if (validation.IsValid)
            return await next(context);
        
        var errors = validation.Errors
            .GroupBy(failure => failure.PropertyName)
            .ToDictionary(group => group.Key, group => group.Select(f => f.ErrorMessage).ToArray());
        
        return Results.ValidationProblem(errors);
    }
}