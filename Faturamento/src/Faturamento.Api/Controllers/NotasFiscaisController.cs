using Faturamento.Application.UseCases.AdicionarItem;
using Faturamento.Application.UseCases.CriarNotaFiscal;
using Faturamento.Application.UseCases.ImprimirNotaFiscal;
using Faturamento.Application.UseCases.RemoverItem;
using Faturamento.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Faturamento.Api.Controllers;

[ApiController]
[Route("api/notas-fiscais")]
public class NotasFiscaisController : ControllerBase
{
    private readonly INotaFiscalRepository _notaFiscalRepository;
    private readonly CriarNotaFiscalUseCase _criarNotaFiscalUseCase;
    private readonly AdicionarItemUseCase _adicionarItemUseCase;
    private readonly ImprimirNotaFiscalUseCase _imprimirNotaFiscalUseCase;
    private readonly RemoverItemUseCase _removerItemUseCase;

    public NotasFiscaisController(
        INotaFiscalRepository notaFiscalRepository,
        CriarNotaFiscalUseCase criarNotaFiscalUseCase,
        AdicionarItemUseCase adicionarItemUseCase,
        ImprimirNotaFiscalUseCase imprimirNotaFiscalUseCase,
        RemoverItemUseCase removerItemUseCase)
    {
        _notaFiscalRepository = notaFiscalRepository;
        _criarNotaFiscalUseCase = criarNotaFiscalUseCase;
        _adicionarItemUseCase = adicionarItemUseCase;
        _imprimirNotaFiscalUseCase = imprimirNotaFiscalUseCase;
        _removerItemUseCase = removerItemUseCase;
    }

    // POST /api/notas-fiscais
    [HttpPost]
    public async Task<ActionResult<CriarNotaFiscalOutput>> Criar([FromBody] CriarNotaFiscalInput input)
    {
        var resultado = await _criarNotaFiscalUseCase.ExecutarAsync(input);
        return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Id }, resultado);
    }

    // GET /api/notas-fiscais
    [HttpGet]
    public async Task<ActionResult> Listar()
    {
        var notas = await _notaFiscalRepository.ListarAsync();

        var resposta = notas.Select(n => new
        {
            n.Id,
            n.Numero,
            Status = n.Status.ToString(),
            n.CriadaEm,
            n.FechadaEm,
            TotalItens = n.Itens.Count
        });

        return Ok(resposta);
    }

    // GET /api/notas-fiscais/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> ObterPorId(Guid id)
    {
        var nota = await _notaFiscalRepository.ObterPorIdAsync(id);
        if (nota is null)
            return NotFound(new { mensagem = $"Nota fiscal com id '{id}' não encontrada." });

        var resposta = new
        {
            nota.Id,
            nota.Numero,
            Status = nota.Status.ToString(),
            nota.CriadaEm,
            nota.FechadaEm,
            Itens = nota.Itens.Select(i => new
            {
                i.Id,
                i.ProdutoId,
                i.ProdutoCodigo,
                i.ProdutoDescricao,
                i.Quantidade
            })
        };

        return Ok(resposta);
    }

    // POST /api/notas-fiscais/{id}/itens
    [HttpPost("{id:guid}/itens")]
    public async Task<ActionResult<AdicionarItemOutput>> AdicionarItem(Guid id, [FromBody] AdicionarItemDto dto)
    {
        var input = new AdicionarItemInput
        {
            NotaFiscalId = id,
            ProdutoId = dto.ProdutoId,
            ProdutoCodigo = dto.ProdutoCodigo,
            ProdutoDescricao = dto.ProdutoDescricao,
            Quantidade = dto.Quantidade
        };

        var resultado = await _adicionarItemUseCase.ExecutarAsync(input);
        return Ok(resultado);
    }

    // POST /api/notas-fiscais/{id}/imprimir
    [HttpPost("{id:guid}/imprimir")]
    public async Task<ActionResult> Imprimir(
     Guid id,
     [FromHeader(Name = "Idempotency-Key")] string idempotencyKey)
    {
        var input = new ImprimirNotaFiscalInput { NotaFiscalId = id };
        var resultado = await _imprimirNotaFiscalUseCase.ExecutarAsync(input);

        if (!resultado.Sucesso)
            return StatusCode(503, resultado);

        return Ok(resultado);
    }

    // DELETE /api/notas-fiscais/{id}/itens/{itemId}
    [HttpDelete("{id:guid}/itens/{itemId:guid}")]
    public async Task<IActionResult> RemoverItem(Guid id, Guid itemId)
    {
        await _removerItemUseCase.ExecutarAsync(new RemoverItemInput
        {
            NotaFiscalId = id,
            ItemId = itemId
        });

        return NoContent();
    }
}

// DTO auxiliar: evita expor NotaFiscalId no corpo, já que ele já vem na rota (id)
public class AdicionarItemDto
{
    public Guid ProdutoId { get; set; }
    public string ProdutoCodigo { get; set; } = string.Empty;
    public string ProdutoDescricao { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}