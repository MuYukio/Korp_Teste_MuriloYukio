using Faturamento.Domain.Interfaces;

namespace Faturamento.Application.UseCases.AdicionarItem;

public class AdicionarItemUseCase
{
    private readonly INotaFiscalRepository _notaFiscalRepository;

    public AdicionarItemUseCase(INotaFiscalRepository notaFiscalRepository)
    {
        _notaFiscalRepository = notaFiscalRepository;
    }

    public async Task<AdicionarItemOutput> ExecutarAsync(AdicionarItemInput input)
    {
        var notaFiscal = await _notaFiscalRepository.ObterPorIdAsync(input.NotaFiscalId)
            ?? throw new InvalidOperationException($"Nota fiscal '{input.NotaFiscalId}' não encontrada.");

        notaFiscal.AdicionarItem(input.ProdutoId, input.ProdutoCodigo, input.ProdutoDescricao, input.Quantidade);

        await _notaFiscalRepository.AtualizarAsync(notaFiscal);

        return new AdicionarItemOutput
        {
            NotaFiscalId = notaFiscal.Id,
            TotalItens = notaFiscal.Itens.Count
        };
    }
}