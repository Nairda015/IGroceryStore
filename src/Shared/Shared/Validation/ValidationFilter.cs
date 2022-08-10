using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Abstractions;

namespace IGroceryStore.Shared.Validation;

public class ValidationFilter<T> : IRouteHandlerFilter
    where T : class
{
    private readonly IValidator<T> _validator;
    public ValidationFilter(IValidator<T> validator) => _validator = validator;

    public async ValueTask<object?> InvokeAsync(RouteHandlerInvocationContext context,
        RouteHandlerFilterDelegate next)
    {
        if (context.Arguments.SingleOrDefault(x => x?.GetType() == typeof(T)) is not T validatable)
            return Results.BadRequest();

        var result = await _validator.ValidateAsync(validatable);
        if (!result.IsValid) return Results.BadRequest(result);
        
        return await next(context);
    }
}