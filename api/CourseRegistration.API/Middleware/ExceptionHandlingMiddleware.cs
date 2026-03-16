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

        var httpResponse = context.Response;
        httpResponse.ContentType = "application/json";

        var apiErrorResponse = new ApiResponseDto<object>
        {
            Success = false,
            Data = null
        };

        switch (exception)
        {
            case ArgumentException argEx:
                httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                apiErrorResponse.Message = "Invalid argument provided";
                apiErrorResponse.Errors = new[] { argEx.Message };
                break;

            case InvalidOperationException invOpEx:
                httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                apiErrorResponse.Message = "Invalid operation";
                apiErrorResponse.Errors = new[] { invOpEx.Message };
                break;

            case UnauthorizedAccessException:
                httpResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                apiErrorResponse.Message = "Unauthorized access";
                apiErrorResponse.Errors = new[] { "You are not authorized to perform this action" };
                break;

            case KeyNotFoundException:
                httpResponse.StatusCode = (int)HttpStatusCode.NotFound;
                apiErrorResponse.Message = "Resource not found";
                apiErrorResponse.Errors = new[] { "The requested resource was not found" };
                break;

            case TimeoutException:
                httpResponse.StatusCode = (int)HttpStatusCode.RequestTimeout;
                apiErrorResponse.Message = "Request timeout";
                apiErrorResponse.Errors = new[] { "The request timed out" };
                break;

            default:
                httpResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                apiErrorResponse.Message = "An internal server error occurred";
                
                // In development, include the full exception details
                if (context.RequestServices.GetService<IWebHostEnvironment>()?.IsDevelopment() == true)
                {
                    apiErrorResponse.Errors = new[] { exception.Message, exception.StackTrace ?? string.Empty };
                }
                else
                {
                    apiErrorResponse.Errors = new[] { "Please contact support with correlation ID: " + correlationId };
                }
                break;
        }

        var camelCaseJsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var jsonResponse = JsonSerializer.Serialize(apiErrorResponse, camelCaseJsonOptions);
        await httpResponse.WriteAsync(jsonResponse);
    }
}