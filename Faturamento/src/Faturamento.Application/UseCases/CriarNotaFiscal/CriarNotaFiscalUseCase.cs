using Faturamento.Domain.Entities;
using Faturamento.Domain.Interfaces;

namespace Faturamento.Application.UseCases.CriarNotaFiscal;

public class CriarNotaFiscalUseCase
{
    private readonly INotaFiscalRepository _notaFiscalRepository;

    public CriarNotaFiscalUseCase(INotaFiscalRepository notaFiscalRepository)
    {
        _notaFiscalRepository = notaFiscalRepository;
    }

    public async Task<CriarNotaFiscalOutput> ExecutarAsync(CriarNotaFiscalInput input)
    {
        var proximoNumero = await _notaFiscalRepository.ObterProximoNumeroAsync();

        var notaFiscal = new NotaFiscal(proximoNumero);

        await _notaFiscalRepository.AdicionarAsync(notaFiscal);

        return new CriarNotaFiscalOutput
        {
            Id = notaFiscal.Id,
            Numero = notaFiscal.Numero,
            Status = notaFiscal.Status.ToString(),
            CriadaEm = notaFiscal.CriadaEm
        };
    }
}