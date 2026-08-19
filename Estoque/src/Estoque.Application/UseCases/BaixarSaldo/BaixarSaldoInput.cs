namespace Estoque.Application.UseCases.BaixarSaldo;

public class BaixarSaldoInput
{
    public List<ItemBaixaInput> Itens { get; set; } = new();
}

public class ItemBaixaInput
{
    public string Codigo { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}