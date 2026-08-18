using Estoque.Domain.Exceptions;

namespace Estoque.Domain.Entities;

public class Produto
{
    // Construtor privado: EF Core precisa dele para materializar do banco,
    // mas o código da aplicação não pode criar um Produto "vazio" por aqui.
    private Produto() { }

    public Produto(string codigo, string descricao, int saldoInicial)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException("Código do produto é obrigatório.", nameof(codigo));

        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descrição do produto é obrigatória.", nameof(descricao));

        if (saldoInicial < 0)
            throw new ArgumentException("Saldo inicial não pode ser negativo.", nameof(saldoInicial));

        Id = Guid.NewGuid();
        Codigo = codigo;
        Descricao = descricao;
        Saldo = saldoInicial;
        CriadoEm = DateTime.UtcNow;
        AtualizadoEm = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public string Codigo { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public int Saldo { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public DateTime AtualizadoEm { get; private set; }

    /// <summary>
    /// Baixa uma quantidade do saldo do produto.
    /// Lança SaldoInsuficienteException se não houver saldo suficiente.
    /// </summary>
    public void BaixarSaldo(int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade a baixar deve ser maior que zero.", nameof(quantidade));

        if (quantidade > Saldo)
            throw new SaldoInsuficienteException(Codigo, Saldo, quantidade);

        Saldo -= quantidade;
        AtualizadoEm = DateTime.UtcNow;
    }

    /// <summary>
    /// Atualiza descrição do produto (código não é alterável após criação).
    /// </summary>
    public void AtualizarDescricao(string novaDescricao)
    {
        if (string.IsNullOrWhiteSpace(novaDescricao))
            throw new ArgumentException("Descrição não pode ser vazia.", nameof(novaDescricao));

        Descricao = novaDescricao;
        AtualizadoEm = DateTime.UtcNow;
    }
}