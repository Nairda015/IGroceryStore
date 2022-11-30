using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;
using IGroceryStore.Shared.Exceptions;

namespace IGroceryStore.API.Middlewares;

internal sealed class ExceptionMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionMiddleware> _logger;
    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (UnreachableException ex)
        {
            context.Response.StatusCode = 500;
            context.Response.Headers.Add("content-type", "application/json");

            if (ex.InnerException is null)
            {
                var json = JsonSerializer.Serialize(new
                {
                    ErrorCode = nameof(UnreachableException), 
                    Message = "Something went wrong. Please contact support."
                });
                await context.Response.WriteAsync(json);
            }
            else
            {
                var errorCode = ToUnderscoreCase(ex.InnerException.GetType().Name.Replace("Exception", string.Empty));
                var json = JsonSerializer.Serialize(new { ErrorCode = errorCode, ex.InnerException.Message });
                await context.Response.WriteAsync(json);
            }

            _logger.LogCritical(ex, "UnreachableException Message: {Message}",  ex.Message);
        }
        catch (GroceryStoreException ex)
        {
            context.Response.StatusCode = (int)ex.StatusCode;
            context.Response.Headers.Add("content-type", "application/json");

            var errorCode = ToUnderscoreCase(ex.GetType().Name.Replace("Exception", string.Empty));
            var json = JsonSerializer.Serialize(new { ErrorCode = errorCode, ex.Message });
            await context.Response.WriteAsync(json);
        }
        catch (GroceryConsumerException ex)
        {
            if (ex.ShouldBeRetried)
            {
                _logger.LogWarning(ex, "{Exception} with id: {Id} will be resent. Retry number: {RetryNumber}", 
                    ex.GetType().Name,
                    ex.CorrelationId,
                    ex.RetryCount);
                throw;
            }

            _logger.LogCritical(ex, "{Exception} with id: {Id} will not be resent",
                ex.GetType().Name,
                ex.CorrelationId);
            throw;
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
            
            _logger.LogCritical(ex, "{Exception} Message: {Message}", ex.GetType().Name, ex.Message);
        }
    }
    private static string ToUnderscoreCase(string value)
        => string.Concat(value.Select((x, i) => i > 0 && char.IsUpper(x) && !char.IsUpper(value[i-1]) ? $"_{x}" : x.ToString())).ToLower();
}
