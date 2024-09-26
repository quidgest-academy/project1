using CSGenio.framework;

namespace GenioMVC;

/// <summary>
/// Low level catch all for unhandled exceptions
/// Creates error 500 responses with the message and callstack in the body
/// </summary>
public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);

            context.Response.StatusCode = 500;
            context.Response.Headers.ContentType = "text/plain";
            await using (var writer = new StreamWriter(context.Response.Body))
            {
                if (Configuration.EventTracking)
                {
                    writer.WriteLine(ex.Message);
                    writer.WriteLine();
                    writer.WriteLine(ex.StackTrace);
                }
                else
                {
                    writer.WriteLine("A low level exception has ocurred. Please review server application logs for details.");
                }
            }

            return;
        }
    }
}
