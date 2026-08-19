using Estoque.Domain.Entities;
using Estoque.Domain.Interfaces;
using Estoque.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Infrastructure.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly EstoqueDbContext _context;

    public ProdutoRepository(EstoqueDbContext context)
    {
        _context = context;
    }

    public async Task<Produto?> ObterPorIdAsync(Guid id)
    {
        return await _context.Produtos
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Produto?> ObterPorCodigoAsync(string codigo)
    {
        return await _context.Produtos
            .FirstOrDefaultAsync(p => p.Codigo == codigo);
    }

    public async Task<List<Produto>> ListarAsync()
    {
        return await _context.Produtos
            .OrderBy(p => p.Codigo)
            .ToListAsync();
    }

    public async Task AdicionarAsync(Produto produto)
    {
        await _context.Produtos.AddAsync(produto);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Produto produto)
    {
        _context.Produtos.Update(produto);
        await _context.SaveChangesAsync();
    }
}