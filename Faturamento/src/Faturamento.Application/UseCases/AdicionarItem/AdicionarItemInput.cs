namespace Faturamento.Application.UseCases.AdicionarItem;

public class AdicionarItemInput
{
    public Guid NotaFiscalId { get; set; }
    public Guid ProdutoId { get; set; }
    public string ProdutoCodigo { get; set; } = string.Empty;
    public string ProdutoDescricao { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}