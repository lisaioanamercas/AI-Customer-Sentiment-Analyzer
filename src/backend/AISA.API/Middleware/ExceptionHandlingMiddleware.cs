using System.Net;
using System.Text.Json;
using FluentValidation;

namespace AISA.API.Middleware;

/// <summary>
/// Middleware global pentru tratarea excepțiilor.
/// Convertește excepțiile în răspunsuri HTTP standardizate.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("Validare eșuată: {Errors}", ex.Errors);
            await WriteErrorResponse(context, HttpStatusCode.BadRequest, "Erori de validare", ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Resursă negăsită: {Message}", ex.Message);
            await WriteErrorResponse(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Operație invalidă: {Message}", ex.Message);
            await WriteErrorResponse(context, HttpStatusCode.BadGateway, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Eroare neașteptată: {Message}", ex.Message);
            await WriteErrorResponse(context, HttpStatusCode.InternalServerError, "A apărut o eroare internă.");
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, HttpStatusCode statusCode, string message, IEnumerable<string>? errors = null)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            status = (int)statusCode,
            message,
            errors = errors ?? Enumerable.Empty<string>(),
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
