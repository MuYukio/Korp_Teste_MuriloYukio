using Estoque.Domain.Interfaces;

namespace Estoque.Application.UseCases.BaixarSaldo;

public class BaixarSaldoUseCase
{
    private readonly IProdutoRepository _produtoRepository;

    public BaixarSaldoUseCase(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<BaixarSaldoOutput> ExecutarAsync(BaixarSaldoInput input)
    {
        var output = new BaixarSaldoOutput { Sucesso = true };

        foreach (var item in input.Itens)
        {
            var produto = await _produtoRepository.ObterPorCodigoAsync(item.Codigo);

            if (produto is null)
            {
                output.Sucesso = false;
                output.Erros.Add($"Produto '{item.Codigo}' não encontrado.");
                continue;
            }

            produto.BaixarSaldo(item.Quantidade);

            await _produtoRepository.AtualizarAsync(produto);
        }

        return output;
    }
}