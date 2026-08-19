using Faturamento.Domain.Entities;

namespace Faturamento.Domain.Interfaces;

public interface INotaFiscalRepository
{
    Task<NotaFiscal?> ObterPorIdAsync(Guid id);
    Task<List<NotaFiscal>> ListarAsync();
    Task<int> ObterProximoNumeroAsync();
    Task AdicionarAsync(NotaFiscal notaFiscal);
    Task AtualizarAsync(NotaFiscal notaFiscal);
}