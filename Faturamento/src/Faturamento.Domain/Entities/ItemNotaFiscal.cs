namespace Faturamento.Domain.Entities;

public class ItemNotaFiscal
{
    private ItemNotaFiscal() { }

    public ItemNotaFiscal(Guid produtoId, string produtoCodigo, string produtoDescricao, int quantidade)
    {
        if (produtoId == Guid.Empty)
            throw new ArgumentException("Produto é obrigatório.", nameof(produtoId));

        if (string.IsNullOrWhiteSpace(produtoCodigo))
            throw new ArgumentException("Código do produto é obrigatório.", nameof(produtoCodigo));

        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero.", nameof(quantidade));

        Id = Guid.NewGuid();
        ProdutoId = produtoId;
        ProdutoCodigo = produtoCodigo;
        ProdutoDescricao = produtoDescricao;
        Quantidade = quantidade;
    }

    public Guid Id { get; private set; }
    public Guid NotaFiscalId { get; private set; }
    public Guid ProdutoId { get; private set; }
    public string ProdutoCodigo { get; private set; } = string.Empty;
    public string ProdutoDescricao { get; private set; } = string.Empty;
    public int Quantidade { get; private set; }
}