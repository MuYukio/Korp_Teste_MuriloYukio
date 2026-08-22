namespace Faturamento.Application.Interfaces;

public class BaixarSaldoEstoqueRequest
{
    public List<ItemBaixaEstoque> Itens { get; set; } = new();
}

public class ItemBaixaEstoque
{
    public string Codigo { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}

public class BaixarSaldoEstoqueResponse
{
    public bool Sucesso { get; set; }
    public List<string> Erros { get; set; } = new();
}

public interface IEstoqueApiClient
{
    Task BaixarSaldoAsync(BaixarSaldoEstoqueRequest request);
}