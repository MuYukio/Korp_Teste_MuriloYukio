using System.Net;
using System.Net.Http.Json;
using Faturamento.Application.Interfaces;
using Faturamento.Domain.Exceptions;
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

    public async Task BaixarSaldoAsync(BaixarSaldoEstoqueRequest request)
    {
        HttpResponseMessage response;
        try
        {
            response = await _httpClient.PatchAsJsonAsync("/api/produtos/baixar-saldo", request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha de comunicação com Estoque.Api.");
            throw new EstoqueIndisponivelException(
                "Não foi possível processar. Tente novamente em instantes.", ex);
        }

        if (response.IsSuccessStatusCode)
        {
            return;
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var corpoErro = await response.Content.ReadFromJsonAsync<ErroEstoqueApiResponse>();
            var mensagem = corpoErro?.Erro ?? "Requisição recusada pelo Estoque.Api.";

            _logger.LogWarning("Estoque.Api recusou a operação: {Mensagem}", mensagem);
            throw new OperacaoRecusadaPeloEstoqueException(mensagem);
        }

        _logger.LogWarning("Estoque.Api retornou {StatusCode} inesperado.", response.StatusCode);
        throw new EstoqueIndisponivelException(
            "Não foi possível processar. Tente novamente em instantes.");
    }
}

internal class ErroEstoqueApiResponse
{
    public string? Erro { get; set; }
    public int StatusCode { get; set; }
}