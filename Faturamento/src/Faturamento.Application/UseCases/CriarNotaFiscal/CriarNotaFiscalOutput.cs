namespace Faturamento.Application.UseCases.CriarNotaFiscal;

public class CriarNotaFiscalOutput
{
    public Guid Id { get; set; }
    public int Numero { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CriadaEm { get; set; }
}