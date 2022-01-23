using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using IGroceryStore.Shared.Abstraction.Exceptions;
using Microsoft.AspNetCore.Http;

namespace IGroceryStore.Shared.Exceptions;

internal sealed class ExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (GroceryStoreException ex)
        {
            context.Response.StatusCode = (int) ex.StatusCode;
            context.Response.Headers.Add("content-type", "application/json");

            var errorCode = ToUnderscoreCase(ex.GetType().Name.Replace("Exception", string.Empty));
            var json = JsonSerializer.Serialize(new {ErrorCode = errorCode, ex.Message});
            await context.Response.WriteAsync(json);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = 400;
            context.Response.Headers.Add("content-type", "application/json");

            var errorCode = ToUnderscoreCase(ex.GetType().Name.Replace("Exception", string.Empty));
            var json = JsonSerializer.Serialize(new {ErrorCode = errorCode, ex.Message});
            await context.Response.WriteAsync(json);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            context.Response.Headers.Add("content-type", "application/json");

            var errorCode = ToUnderscoreCase(ex.GetType().Name.Replace("Exception", string.Empty));
            var json = JsonSerializer.Serialize(new {ErrorCode = errorCode, ex.Message});
            await context.Response.WriteAsync(json);
        }
    }
    private static string ToUnderscoreCase(string value)
        => string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) && !char.IsUpper(value[i-1]) ? $"_{x}" : x.ToString())).ToLower();
}