namespace Faturamento.Application.UseCases.ImprimirNotaFiscal;

public class ImprimirNotaFiscalOutput
{
    public bool Sucesso { get; set; }
    public Guid NotaFiscalId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Mensagem { get; set; }
}