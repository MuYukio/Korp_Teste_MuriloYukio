namespace Estoque.Application.UseCases.CadastrarProduto;

public class CadastrarProdutoInput
{
    public string Codigo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int SaldoInicial { get; set; }
}