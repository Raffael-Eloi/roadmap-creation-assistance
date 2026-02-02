using System.Text.Json;

namespace RoadmapCreationAssistance.API.Middlewares;

public class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ArgumentException ex)
        {
            object response = CreateResponse(ex, context, StatusCodes.Status422UnprocessableEntity);
            await context.Response.WriteAsJsonAsync(response);
        }
        catch (JsonException ex)
        {
            object response = CreateResponse(ex, context, StatusCodes.Status422UnprocessableEntity);
            await context.Response.WriteAsJsonAsync(response);
        }
        catch (HttpRequestException ex)
        {
            object response = CreateResponse(ex, context, StatusCodes.Status422UnprocessableEntity);
            await context.Response.WriteAsJsonAsync(response);
        }
        catch (InvalidOperationException ex)
        {
            object response = CreateResponse(ex, context, StatusCodes.Status422UnprocessableEntity);
            await context.Response.WriteAsJsonAsync(response);
        }
        catch (Exception ex)
        {
            object response = CreateResponse(ex, context, StatusCodes.Status400BadRequest);
            await context.Response.WriteAsJsonAsync(response);
        }
    }

    private object CreateResponse(Exception ex, HttpContext context, int statusCode)
    {
        logger.LogError(ex, "A handled exception occurred: {Message}", ex.Message);
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        return new
        {
            ex.Message
        };
    }
}