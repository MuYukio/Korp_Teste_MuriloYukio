namespace Estoque.Application.Interfaces;

public class SugestaoProdutoResponse
{
    public string Descricao { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
}

public interface IIaClient
{
    Task<SugestaoProdutoResponse> SugerirDescricaoECategoriaAsync(string codigoProduto);
}