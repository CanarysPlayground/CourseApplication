using System.Net;
using System.Text.Json;
using CourseRegistration.Application.DTOs;

namespace CourseRegistration.API.Middleware;

/// <summary>
/// Global exception handling middleware
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the ExceptionHandlingMiddleware
    /// </summary>
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Invokes the middleware
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Handles exceptions and returns appropriate HTTP responses
    /// </summary>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = context.TraceIdentifier;
        
        _logger.LogError(exception, "An unhandled exception occurred. CorrelationId: {CorrelationId}", correlationId);

        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ApiResponseDto<object>
        {
            Success = false,
            Data = null
        };

        switch (exception)
        {
            case ArgumentException argEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Invalid argument provided";
                errorResponse.Errors = new[] { argEx.Message };
                break;

            case InvalidOperationException invOpEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = "Invalid operation";
                errorResponse.Errors = new[] { invOpEx.Message };
                break;

            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Message = "Unauthorized access";
                errorResponse.Errors = new[] { "You are not authorized to perform this action" };
                break;

            case KeyNotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Message = "Resource not found";
                errorResponse.Errors = new[] { "The requested resource was not found" };
                break;

            case TimeoutException:
                response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                errorResponse.Message = "Request timeout";
                errorResponse.Errors = new[] { "The request timed out" };
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = "An internal server error occurred";
                
                // In development, include the full exception details
                if (context.RequestServices.GetService<IWebHostEnvironment>()?.IsDevelopment() == true)
                {
                    errorResponse.Errors = new[] { exception.Message, exception.StackTrace ?? string.Empty };
                }
                else
                {
                    errorResponse.Errors = new[] { "Please contact support with correlation ID: " + correlationId };
                }
                break;
        }

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var jsonResponse = JsonSerializer.Serialize(errorResponse, jsonOptions);
        await response.WriteAsync(jsonResponse);
    }
}