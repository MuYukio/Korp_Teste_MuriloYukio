using Estoque.Application.Interfaces;

namespace Estoque.Application.UseCases.SugerirDescricaoProduto;

public class SugerirDescricaoProdutoUseCase
{
    private readonly IIaClient _iaClient;

    public SugerirDescricaoProdutoUseCase(IIaClient iaClient)
    {
        _iaClient = iaClient;
    }

    public async Task<SugerirDescricaoProdutoOutput> ExecutarAsync(SugerirDescricaoProdutoInput input)
    {
        if (string.IsNullOrWhiteSpace(input.CodigoProduto))
            throw new ArgumentException("Código do produto é obrigatório para gerar sugestão.", nameof(input));

        var sugestao = await _iaClient.SugerirDescricaoECategoriaAsync(input.CodigoProduto);

        return new SugerirDescricaoProdutoOutput
        {
            Descricao = sugestao.Descricao,
            Categoria = sugestao.Categoria
        };
    }
}