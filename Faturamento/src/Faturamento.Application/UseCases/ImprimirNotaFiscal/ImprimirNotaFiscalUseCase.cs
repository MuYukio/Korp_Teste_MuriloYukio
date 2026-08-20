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

        // Fecher() já valida status == Aberta e itens.Count > 0 — lança
        // NotaFiscalInvalidaException se algo estiver errado, capturada pelo middleware.
        // Chamamos ANTES de tentar baixar o saldo, pra falhar rápido sem nem
        // acionar o Estoque se a nota já não puder ser impressa.
        notaFiscal.ValidarPodeSerFechada();

        var request = new BaixarSaldoEstoqueRequest
        {
            Itens = notaFiscal.Itens
                .Select(i => new ItemBaixaEstoque { Codigo = i.ProdutoCodigo, Quantidade = i.Quantidade })
                .ToList()
        };

        var resultadoBaixa = await _estoqueApiClient.BaixarSaldoAsync(request);

        if (!resultadoBaixa.Sucesso)
        {
            // Fallback: a nota permanece Aberta, nunca fica em estado inconsistente.
            // Retornamos Sucesso = false para o Controller decidir o status HTTP (503).
            return new ImprimirNotaFiscalOutput
            {
                Sucesso = false,
                NotaFiscalId = notaFiscal.Id,
                Status = notaFiscal.Status.ToString(),
                Mensagem = "Não foi possível processar. Tente novamente em instantes."
            };
        }

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