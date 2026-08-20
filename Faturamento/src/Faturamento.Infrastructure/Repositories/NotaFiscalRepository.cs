using Faturamento.Domain.Entities;
using Faturamento.Domain.Interfaces;
using Faturamento.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Faturamento.Infrastructure.Repositories;

public class NotaFiscalRepository : INotaFiscalRepository
{
    private readonly FaturamentoDbContext _context;

    public NotaFiscalRepository(FaturamentoDbContext context)
    {
        _context = context;
    }

    public async Task<NotaFiscal?> ObterPorIdAsync(Guid id)
    {
        return await _context.NotasFiscais
            .Include(n => n.Itens)
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<List<NotaFiscal>> ListarAsync()
    {
        return await _context.NotasFiscais
            .Include(n => n.Itens)
            .OrderByDescending(n => n.Numero)
            .ToListAsync();
    }

    public async Task<int> ObterProximoNumeroAsync()
    {
        var maiorNumero = await _context.NotasFiscais
            .Select(n => (int?)n.Numero)
            .MaxAsync();

        return (maiorNumero ?? 0) + 1;
    }

    public async Task AdicionarAsync(NotaFiscal notaFiscal)
    {
        await _context.NotasFiscais.AddAsync(notaFiscal);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(NotaFiscal notaFiscal)
    {
        var idsExistentes = await _context.ItensNotaFiscal
            .Where(i => i.NotaFiscalId == notaFiscal.Id)
            .Select(i => i.Id)
            .ToListAsync();

        foreach (var item in notaFiscal.Itens)
        {
            if (!idsExistentes.Contains(item.Id))
            {
                _context.Entry(item).State = EntityState.Added;
            }
        }

        await _context.SaveChangesAsync();
    }
}