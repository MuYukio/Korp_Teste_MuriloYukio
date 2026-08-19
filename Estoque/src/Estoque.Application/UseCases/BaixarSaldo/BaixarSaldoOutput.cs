namespace Estoque.Application.UseCases.BaixarSaldo;

public class BaixarSaldoOutput
{
    public bool Sucesso { get; set; }
    public List<string> Erros { get; set; } = new();
}