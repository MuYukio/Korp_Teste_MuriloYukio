using Faturamento.Domain.Interfaces;

namespace Faturamento.Application.UseCases.RemoverItem;

public class RemoverItemInput
{
    public Guid NotaFiscalId { get; set; }
    public Guid ItemId { get; set; }
}

public class RemoverItemUseCase
{
    private readonly INotaFiscalRepository _notaFiscalRepository;

    public RemoverItemUseCase(INotaFiscalRepository notaFiscalRepository)
    {
        _notaFiscalRepository = notaFiscalRepository;
    }

    public async Task ExecutarAsync(RemoverItemInput input)
    {
        var notaFiscal = await _notaFiscalRepository.ObterPorIdAsync(input.NotaFiscalId)
            ?? throw new InvalidOperationException($"Nota fiscal '{input.NotaFiscalId}' não encontrada.");

        notaFiscal.RemoverItem(input.ItemId);
        await _notaFiscalRepository.AtualizarAsync(notaFiscal);
    }
}