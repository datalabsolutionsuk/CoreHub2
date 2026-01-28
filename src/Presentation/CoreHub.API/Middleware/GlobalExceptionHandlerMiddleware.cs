using System.Net;
using System.Text.Json;
using FluentValidation;

namespace CoreHub.API.Middleware;

/// <summary>
/// Global exception handling middleware for consistent error responses
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

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

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = Guid.NewGuid().ToString();
        
        _logger.LogError(exception, "Unhandled exception occurred. CorrelationId: {CorrelationId}", correlationId);

        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = exception switch
        {
            ValidationException validationEx => CreateValidationErrorResponse(validationEx, correlationId),
            KeyNotFoundException => CreateErrorResponse(
                HttpStatusCode.NotFound,
                "Resource not found",
                exception.Message,
                correlationId),
            ArgumentException => CreateErrorResponse(
                HttpStatusCode.BadRequest,
                "Invalid argument",
                exception.Message,
                correlationId),
            InvalidOperationException invalidOpEx => CreateInvalidOperationResponse(invalidOpEx, correlationId),
            UnauthorizedAccessException => CreateErrorResponse(
                HttpStatusCode.Unauthorized,
                "Unauthorized",
                "You are not authorized to perform this action",
                correlationId),
            _ => CreateErrorResponse(
                HttpStatusCode.InternalServerError,
                "Internal server error",
                _environment.IsDevelopment() ? exception.Message : "An unexpected error occurred",
                correlationId)
        };

        response.StatusCode = errorResponse.StatusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        };

        await response.WriteAsync(JsonSerializer.Serialize(errorResponse, options));
    }

    private static ApiErrorResponse CreateValidationErrorResponse(ValidationException ex, string correlationId)
    {
        var errors = ex.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage)).ToList();
        
        return new ApiErrorResponse
        {
            StatusCode = (int)HttpStatusCode.BadRequest,
            Title = "Validation failed",
            Message = "One or more validation errors occurred",
            CorrelationId = correlationId,
            ValidationErrors = errors
        };
    }

    private static ApiErrorResponse CreateInvalidOperationResponse(InvalidOperationException ex, string correlationId)
    {
        var statusCode = ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase)
            ? HttpStatusCode.NotFound
            : HttpStatusCode.BadRequest;

        return CreateErrorResponse(statusCode, "Operation failed", ex.Message, correlationId);
    }

    private static ApiErrorResponse CreateErrorResponse(
        HttpStatusCode statusCode,
        string title,
        string message,
        string correlationId)
    {
        return new ApiErrorResponse
        {
            StatusCode = (int)statusCode,
            Title = title,
            Message = message,
            CorrelationId = correlationId
        };
    }
}

/// <summary>
/// Standardized API error response
/// </summary>
public class ApiErrorResponse
{
    /// <summary>
    /// HTTP status code
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Error title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Error message with details
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Correlation ID for tracking
    /// </summary>
    public string CorrelationId { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the error occurred
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Validation errors (if applicable)
    /// </summary>
    public List<ValidationError>? ValidationErrors { get; set; }
}

/// <summary>
/// Validation error detail
/// </summary>
public class ValidationError
{
    public ValidationError(string field, string message)
    {
        Field = field;
        Message = message;
    }

    /// <summary>
    /// Field that failed validation
    /// </summary>
    public string Field { get; set; }

    /// <summary>
    /// Validation error message
    /// </summary>
    public string Message { get; set; }
}

/// <summary>
/// Extension methods for registering the global exception handler
/// </summary>
public static class GlobalExceptionHandlerMiddlewareExtensions
{
    /// <summary>
    /// Adds global exception handling middleware to the pipeline
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}
