using System.Net;
using System.Text.Json;
using Estoque.Domain.Exceptions;

namespace Estoque.Api.Middlewares;

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado capturado pelo middleware.");
            await TratarExcecaoAsync(context, ex);
        }
    }

    private static Task TratarExcecaoAsync(HttpContext context, Exception exception)
    {
        var (statusCode, mensagem) = exception switch
        {
            SaldoInsuficienteException saldoEx => (
                HttpStatusCode.BadRequest,
                saldoEx.Message),

            IaIndisponivelException iaEx => (
                HttpStatusCode.ServiceUnavailable,
                iaEx.Message),

            InvalidOperationException invalidOpEx => (
                HttpStatusCode.Conflict,
                invalidOpEx.Message),

            ArgumentException argEx => (
                HttpStatusCode.BadRequest,
                argEx.Message),

            _ => (
                HttpStatusCode.InternalServerError,
                "Ocorreu um erro inesperado. Tente novamente mais tarde.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var resposta = JsonSerializer.Serialize(new
        {
            erro = mensagem,
            statusCode = (int)statusCode
        });

        return context.Response.WriteAsync(resposta);
    }
}