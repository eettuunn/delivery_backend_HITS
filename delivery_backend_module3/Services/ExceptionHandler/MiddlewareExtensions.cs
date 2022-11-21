namespace delivery_backend_module3.Services.ExceptionHandler;

public static class MiddlewareExtensions
{
    public static void UseExceptionHandlingMiddlwares(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddlewareService>();
    }
}