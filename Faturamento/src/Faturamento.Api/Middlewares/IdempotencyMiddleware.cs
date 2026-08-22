using Faturamento.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Faturamento.Api.Middlewares;

public class IdempotencyMiddleware
{
    private const string HeaderChave = "Idempotency-Key";
    private const string RotaProtegida = "/api/notas-fiscais";
    private const string SegmentoImprimir = "/imprimir";

    private readonly RequestDelegate _next;
    private readonly ILogger<IdempotencyMiddleware> _logger;

    public IdempotencyMiddleware(RequestDelegate next, ILogger<IdempotencyMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, FaturamentoDbContext dbContext)
    {

        var ehImpressao = HttpMethods.IsPost(context.Request.Method)
            && context.Request.Path.StartsWithSegments(RotaProtegida)
            && context.Request.Path.Value!.EndsWith(SegmentoImprimir, StringComparison.OrdinalIgnoreCase);

        if (!ehImpressao)
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(HeaderChave, out var chaveHeader) || string.IsNullOrWhiteSpace(chaveHeader))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                """{"erro": "Header 'Idempotency-Key' é obrigatório para esta operação.", "statusCode": 400}""");
            return;
        }

        var chave = chaveHeader.ToString();

        var registroExistente = await dbContext.ChavesIdempotencia
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Chave == chave);

        if (registroExistente is not null)
        {
            _logger.LogInformation("Chave de idempotência '{Chave}' já processada. Devolvendo resposta salva.", chave);
            context.Response.StatusCode = registroExistente.StatusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(registroExistente.CorpoResposta);
            return;
        }
        var streamOriginal = context.Response.Body;
        using var streamMemoria = new MemoryStream();
        context.Response.Body = streamMemoria;

        try
        {
            await _next(context);

            streamMemoria.Seek(0, SeekOrigin.Begin);
            var corpoResposta = await new StreamReader(streamMemoria).ReadToEndAsync();

            if (context.Response.StatusCode != StatusCodes.Status503ServiceUnavailable)
            {
                dbContext.ChavesIdempotencia.Add(new ChaveIdempotencia
                {
                    Chave = chave,
                    StatusCode = context.Response.StatusCode,
                    CorpoResposta = corpoResposta,
                    CriadaEm = DateTime.UtcNow
                });

                await dbContext.SaveChangesAsync();
            }

            streamMemoria.Seek(0, SeekOrigin.Begin);
            await streamMemoria.CopyToAsync(streamOriginal);
        }
        finally
        {
            context.Response.Body = streamOriginal;
        }
    }
}