using Estoque.Application.Interfaces;
using Estoque.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Estoque.Infrastructure.ExternalServices;

public class GeminiIaClient : IIaClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GeminiIaClient> _logger;

    public GeminiIaClient(HttpClient httpClient, IConfiguration configuration, ILogger<GeminiIaClient> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<SugestaoProdutoResponse> SugerirDescricaoECategoriaAsync(string codigoProduto)
    {
        var apiKey = _configuration["GeminiApi:ApiKey"];
        var baseUrl = _configuration["GeminiApi:BaseUrl"];

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("Chave da API do Gemini não configurada (GeminiApi:ApiKey).");

        var prompt = $$"""
            Você é um assistente de cadastro de produtos para um sistema de estoque.
            Dado o código de um produto, sugira uma descrição curta e uma categoria.

            Código do produto: "{{codigoProduto}}"

            Responda APENAS com um JSON no formato exato:
            {"descricao": "...", "categoria": "..."}

            A descrição deve ter no máximo 60 caracteres.
            A categoria deve ser uma palavra ou expressão curta (ex: "Ferragens", "Eletrônicos", "Limpeza", "Escritório").
            Se o código não der pistas claras do que é o produto, faça sua melhor suposição plausível.
            """;

        var requestBody = new
        {
            contents = new[]
            {
                new { parts = new[] { new { text = prompt } } }
            },
            generationConfig = new
            {
                responseMimeType = "application/json"
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, baseUrl);
        request.Headers.Add("X-goog-api-key", apiKey);
        request.Content = JsonContent.Create(requestBody);

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex) when (ex is TaskCanceledException or HttpRequestException)
        {
            _logger.LogWarning(ex, "Falha ao comunicar com o Gemini para o código '{Codigo}'.", codigoProduto);
            throw new IaIndisponivelException(
                "Não foi possível gerar a sugestão no momento. Tente novamente.", ex);
        }

        var responseJson = await response.Content.ReadFromJsonAsync<GeminiApiResponse>();
        var textoGerado = responseJson?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

        if (string.IsNullOrWhiteSpace(textoGerado))
        {
            _logger.LogWarning("Gemini retornou resposta vazia para o código '{Codigo}'.", codigoProduto);
            throw new IaIndisponivelException("Não foi possível gerar sugestão no momento.");
        }

        var sugestao = JsonSerializer.Deserialize<SugestaoProdutoResponse>(
            textoGerado,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return sugestao ?? throw new IaIndisponivelException("Resposta da IA em formato inesperado.");
    }
    private class GeminiApiResponse
    {
        [JsonPropertyName("candidates")]
        public List<GeminiCandidate>? Candidates { get; set; }
    }

    private class GeminiCandidate
    {
        [JsonPropertyName("content")]
        public GeminiContent? Content { get; set; }
    }

    private class GeminiContent
    {
        [JsonPropertyName("parts")]
        public List<GeminiPart>? Parts { get; set; }
    }

    private class GeminiPart
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}