using Faturamento.Application.Interfaces;
using Faturamento.Domain.Interfaces;

namespace Faturamento.Application.UseCases.ImprimirNotaFiscal;

public class ImprimirNotaFiscalUseCase
{
    private readonly INotaFiscalRepository _notaFiscalRepository;
    private readonly IEstoqueApiClient _estoqueApiClient;

    public ImprimirNotaFiscalUseCase(
        INotaFiscalRepository notaFiscalRepository,
        IEstoqueApiClient estoqueApiClient)
    {
        _notaFiscalRepository = notaFiscalRepository;
        _estoqueApiClient = estoqueApiClient;
    }

    public async Task<ImprimirNotaFiscalOutput> ExecutarAsync(ImprimirNotaFiscalInput input)
    {
        var notaFiscal = await _notaFiscalRepository.ObterPorIdAsync(input.NotaFiscalId)
            ?? throw new InvalidOperationException($"Nota fiscal '{input.NotaFiscalId}' não encontrada.");

        notaFiscal.ValidarPodeSerFechada();

        var request = new BaixarSaldoEstoqueRequest
        {
            Itens = notaFiscal.Itens
                .Select(i => new ItemBaixaEstoque { Codigo = i.ProdutoCodigo, Quantidade = i.Quantidade })
                .ToList()
        };

        await _estoqueApiClient.BaixarSaldoAsync(request);

        notaFiscal.Fechar();
        await _notaFiscalRepository.AtualizarAsync(notaFiscal);

        return new ImprimirNotaFiscalOutput
        {
            Sucesso = true,
            NotaFiscalId = notaFiscal.Id,
            Status = notaFiscal.Status.ToString(),
            Mensagem = "Nota fiscal impressa com sucesso."
        };
    }
}