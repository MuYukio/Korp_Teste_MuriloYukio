using Estoque.Application.DTOs;
using Estoque.Application.UseCases.BaixarSaldo;
using Estoque.Application.UseCases.CadastrarProduto;
using Estoque.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.Api.Controllers;

[ApiController]
[Route("api/produtos")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly CadastrarProdutoUseCase _cadastrarProdutoUseCase;
    private readonly BaixarSaldoUseCase _baixarSaldoUseCase;

    public ProdutosController(
        IProdutoRepository produtoRepository,
        CadastrarProdutoUseCase cadastrarProdutoUseCase,
        BaixarSaldoUseCase baixarSaldoUseCase)
    {
        _produtoRepository = produtoRepository;
        _cadastrarProdutoUseCase = cadastrarProdutoUseCase;
        _baixarSaldoUseCase = baixarSaldoUseCase;
    }

    // POST /api/produtos
    [HttpPost]
    public async Task<ActionResult<CadastrarProdutoOutput>> Cadastrar([FromBody] CadastrarProdutoInput input)
    {
        var resultado = await _cadastrarProdutoUseCase.ExecutarAsync(input);
        return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Id }, resultado);
    }

    // GET /api/produtos
    [HttpGet]
    public async Task<ActionResult<List<ProdutoResponse>>> Listar()
    {
        var produtos = await _produtoRepository.ListarAsync();
        var resposta = produtos.Select(ProdutoResponse.DeEntidade).ToList();
        return Ok(resposta);
    }

    // GET /api/produtos/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProdutoResponse>> ObterPorId(Guid id)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(id);
        if (produto is null)
            return NotFound(new { mensagem = $"Produto com id '{id}' não encontrado." });

        return Ok(ProdutoResponse.DeEntidade(produto));
    }

    // PATCH /api/produtos/baixar-saldo
    [HttpPatch("baixar-saldo")]
    public async Task<ActionResult<BaixarSaldoOutput>> BaixarSaldo([FromBody] BaixarSaldoInput input)
    {
        var resultado = await _baixarSaldoUseCase.ExecutarAsync(input);

        if (!resultado.Sucesso)
            return BadRequest(resultado);

        return Ok(resultado);
    }
}