using Polly;
using Polly.Extensions.Http;

namespace Faturamento.Api.Resilience;

public static class PollyPolicies
{
    /// <summary>
    /// Retry com backoff exponencial: tenta 3 vezes, esperando 2s, 4s, 8s entre tentativas.
    /// Só reage a falhas transitórias (timeout, erro de rede, respostas 5xx) — nunca
    /// reexecuta em caso de erro 4xx, já que isso indicaria um problema no request em si,
    /// não uma falha passageira do serviço.
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> ObterPoliticaRetry()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError() // cobre HttpRequestException e respostas 5xx
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.RequestTimeout)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: tentativa => TimeSpan.FromSeconds(Math.Pow(2, tentativa)));
    }

    /// <summary>
    /// Circuit Breaker: depois de 5 falhas consecutivas, "abre o circuito" por 30 segundos —
    /// durante esse período, novas chamadas falham imediatamente (BrokenCircuitException),
    /// sem sequer tentar acessar a rede. Isso evita sobrecarregar um serviço que já está
    /// claramente fora do ar, dando tempo dele se recuperar.
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> ObterPoliticaCircuitBreaker()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30));
    }
}