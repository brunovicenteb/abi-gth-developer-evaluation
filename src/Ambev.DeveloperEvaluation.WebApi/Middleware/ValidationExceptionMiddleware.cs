using FluentValidation;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware;

public class ValidationExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ValidationExceptionMiddleware> _logger;

    // Error types
    private const string VALIDATION_ERROR_TYPE = "ValidationError";
    private const string NOT_FOUND_ERROR_TYPE = "NotFound";
    private const string UNAUTHORIZED_ERROR_TYPE = "Unauthorized";
    private const string SERVER_ERROR_TYPE = "ServerError";

    // Error messages
    private const string VALIDATION_ERROR_MESSAGE = "Erro ao validar dados.";
    private const string SERVER_ERROR_MESSAGE = "Ocorreu um erro inesperado. Tente novamente mais tarde.";

    // Log messages
    private const string LOG_VALIDATION_ERROR = "Ocorreu um erro de validação.";
    private const string LOG_NOT_FOUND = "Recurso não encontrado.";
    private const string LOG_UNAUTHORIZED = "Acesso não autorizado.";
    private const string LOG_UNHANDLED_EXCEPTION = "Erro inesperado não tratado.";

    public ValidationExceptionMiddleware(RequestDelegate next, ILogger<ValidationExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (KeyNotFoundException ex)
        {
            await HandleKeyNotFoundExceptionAsync(context, ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            await HandleUnauthorizedAccessExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleGenericExceptionAsync(context, ex);
        }
    }

    private async Task HandleValidationExceptionAsync(HttpContext context, ValidationException ex)
    {
        _logger.LogWarning(ex, LOG_VALIDATION_ERROR);

        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = MediaTypeNames.Application.Json;

        var response = new
        {
            type = VALIDATION_ERROR_TYPE,
            message = VALIDATION_ERROR_MESSAGE,
            errors = ex.Errors.Select(e => e.ErrorMessage)
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private async Task HandleKeyNotFoundExceptionAsync(HttpContext context, KeyNotFoundException ex)
    {
        _logger.LogWarning(ex, LOG_NOT_FOUND);

        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        context.Response.ContentType = MediaTypeNames.Application.Json;

        var response = new
        {
            type = NOT_FOUND_ERROR_TYPE,
            message = ex.Message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private async Task HandleUnauthorizedAccessExceptionAsync(HttpContext context, UnauthorizedAccessException ex)
    {
        _logger.LogWarning(ex, LOG_UNAUTHORIZED);

        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        context.Response.ContentType = MediaTypeNames.Application.Json;

        var response = new
        {
            type = UNAUTHORIZED_ERROR_TYPE,
            message = ex.Message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private async Task HandleGenericExceptionAsync(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, LOG_UNHANDLED_EXCEPTION);

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = MediaTypeNames.Application.Json;

        var response = new
        {
            type = SERVER_ERROR_TYPE,
            message = SERVER_ERROR_MESSAGE
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}