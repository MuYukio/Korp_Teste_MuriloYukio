using Faturamento.Domain.Enums;
using Faturamento.Domain.Exceptions;

namespace Faturamento.Domain.Entities;

public class NotaFiscal
{
    private readonly List<ItemNotaFiscal> _itens = new();

    private NotaFiscal() { }

    public NotaFiscal(int numero)
    {
        if (numero <= 0)
            throw new ArgumentException("Número da nota deve ser maior que zero.", nameof(numero));

        Id = Guid.NewGuid();
        Numero = numero;
        Status = StatusNotaFiscal.Aberta;
        CriadaEm = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public int Numero { get; private set; }
    public StatusNotaFiscal Status { get; private set; }
    public DateTime CriadaEm { get; private set; }
    public DateTime? FechadaEm { get; private set; }
    public IReadOnlyCollection<ItemNotaFiscal> Itens => _itens.AsReadOnly();

    public void AdicionarItem(Guid produtoId, string produtoCodigo, string produtoDescricao, int quantidade)
    {
        if (Status != StatusNotaFiscal.Aberta)
            throw new NotaFiscalInvalidaException(
                $"Não é possível adicionar itens a uma nota com status '{Status}'. A nota precisa estar Aberta.");

        var item = new ItemNotaFiscal(produtoId, produtoCodigo, produtoDescricao, quantidade);
        _itens.Add(item);
    }

    public void Fechar()
    {
        if (Status != StatusNotaFiscal.Aberta)
            throw new NotaFiscalInvalidaException(
                $"Não é possível imprimir uma nota com status '{Status}'. Apenas notas Abertas podem ser impressas.");

        if (_itens.Count == 0)
            throw new NotaFiscalInvalidaException("Não é possível imprimir uma nota sem itens.");

        Status = StatusNotaFiscal.Fechada;
        FechadaEm = DateTime.UtcNow;
    }
}