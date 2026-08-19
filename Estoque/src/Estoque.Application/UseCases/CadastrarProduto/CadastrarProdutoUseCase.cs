using Estoque.Domain.Entities;
using Estoque.Domain.Interfaces;

namespace Estoque.Application.UseCases.CadastrarProduto;

public class CadastrarProdutoUseCase
{
    private readonly IProdutoRepository _produtoRepository;

    public CadastrarProdutoUseCase(IProdutoRepository produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<CadastrarProdutoOutput> ExecutarAsync(CadastrarProdutoInput input)
    {
        var produtoExistente = await _produtoRepository.ObterPorCodigoAsync(input.Codigo);
        if (produtoExistente is not null)
            throw new InvalidOperationException($"Já existe um produto cadastrado com o código '{input.Codigo}'.");

        var produto = new Produto(input.Codigo, input.Descricao, input.SaldoInicial);

        await _produtoRepository.AdicionarAsync(produto);

        return new CadastrarProdutoOutput
        {
            Id = produto.Id,
            Codigo = produto.Codigo,
            Descricao = produto.Descricao,
            Saldo = produto.Saldo
        };
    }
}