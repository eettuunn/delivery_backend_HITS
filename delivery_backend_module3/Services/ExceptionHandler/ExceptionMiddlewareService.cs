using delivery_backend_module3.Exceptions;

namespace delivery_backend_module3.Services.ExceptionHandler;

public class ExceptionMiddlewareService
{
    private readonly RequestDelegate _next;

    public ExceptionMiddlewareService(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UserAlreadyExistException exception)
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsJsonAsync(new { message = exception.Message });
        }
        catch (WrongLoginCredentialsException exception)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { message = exception.Message });
        }
        catch (NotAuthorizedException exception)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = exception.Message });
        }
        catch (BadRequestException exception)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { message = exception.Message });
        }
    }
}