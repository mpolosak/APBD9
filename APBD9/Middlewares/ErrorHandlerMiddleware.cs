using APBD9.Exceptions;

namespace APBD9.Middlewares;

public class ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException ex)
        {
            logger.LogError(ex, "Not found");
            await HandleNotFoundAsync(context, ex);
        }
        catch (ConflictException ex)
        {
            logger.LogError(ex, "Conflict");
            await HandleConflictAsync(context, ex);
        }
        catch (BadRequestException ex)
        {
            logger.LogError(ex, "Bad Request");
            await HandleBadRequestAsync(context, ex);
        }
    }

    private static async Task HandleNotFoundAsync(HttpContext context, NotFoundException ex)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        context.Response.ContentType = "application/text";
        await context.Response.WriteAsync(ex.Message);
    }
    
    private static async Task HandleConflictAsync(HttpContext context, ConflictException ex)
    {
        context.Response.StatusCode = StatusCodes.Status409Conflict;
        context.Response.ContentType = "application/text";
        await context.Response.WriteAsync(ex.Message);
    }
    
    public static async Task HandleBadRequestAsync(HttpContext context, BadRequestException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/text";
        await context.Response.WriteAsync(ex.Message);
    }
}