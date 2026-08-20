using System.Net.Http.Json;
using Faturamento.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Faturamento.Infrastructure.ExternalServices;

public class EstoqueApiClient : IEstoqueApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EstoqueApiClient> _logger;

    public EstoqueApiClient(HttpClient httpClient, ILogger<EstoqueApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<BaixarSaldoEstoqueResponse> BaixarSaldoAsync(BaixarSaldoEstoqueRequest request)
    {
        try
        {
            var response = await _httpClient.PatchAsJsonAsync("/api/produtos/baixar-saldo", request);

            if (response.IsSuccessStatusCode)
            {
                var resultado = await response.Content.ReadFromJsonAsync<BaixarSaldoEstoqueResponse>();
                return resultado ?? new BaixarSaldoEstoqueResponse { Sucesso = false, Erros = { "Resposta vazia do Estoque.Api." } };
            }

            _logger.LogWarning("Estoque.Api retornou {StatusCode} ao tentar baixar saldo.", response.StatusCode);
            return new BaixarSaldoEstoqueResponse
            {
                Sucesso = false,
                Erros = { $"Estoque.Api retornou status {(int)response.StatusCode}." }
            };
        }
        catch (Exception ex)
        {
            // Captura falhas de rede, timeout, circuit breaker aberto, etc.
            // O Use Case decide o que fazer com Sucesso = false (mantém a nota Aberta).
            _logger.LogError(ex, "Falha ao comunicar com Estoque.Api ao tentar baixar saldo.");
            return new BaixarSaldoEstoqueResponse
            {
                Sucesso = false,
                Erros = { "Não foi possível processar. Tente novamente em instantes." }
            };
        }
    }
}