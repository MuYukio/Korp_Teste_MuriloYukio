namespace Faturamento.Infrastructure.Persistence;

public class ChaveIdempotencia
{
    public string Chave { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public string CorpoResposta { get; set; } = string.Empty;
    public DateTime CriadaEm { get; set; }
}