using GestaoTarefas.Api.Data;
using GestaoTarefas.Api.Entities;
using GestaoTarefas.Api.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestaoTarefas.Api.Repositories;

public class TarefaRepository : ITarefaRepository
{
    private readonly AppDbContext _context;

    public TarefaRepository(AppDbContext context) => _context = context;

    public async Task<Tarefa> AdicionarAsync(Tarefa tarefa, CancellationToken ct = default)
    {
        await _context.Tarefas.AddAsync(tarefa, ct);
        await _context.SaveChangesAsync(ct);
        return tarefa;
    }

    public async Task<Tarefa?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Tarefas.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IEnumerable<Tarefa>> ListarAsync(
        StatusTarefa? status = null,
        DateTime? dataVencimento = null,
        string? busca = null,
        CancellationToken ct = default)
    {
        var query = _context.Tarefas.AsQueryable();

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (dataVencimento.HasValue)
        {
            var dataAlvo = dataVencimento.Value.Date;
            query = query.Where(t => t.DataVencimento.HasValue && t.DataVencimento.Value.Date == dataAlvo);
        }

        if (!string.IsNullOrWhiteSpace(busca))
        {
            var termo = busca.Trim().ToLowerInvariant();
            query = query.Where(t =>
                t.Titulo.ToLower().Contains(termo) ||
                (t.Descricao != null && t.Descricao.ToLower().Contains(termo)));
        }

        return await query
            .OrderBy(t => t.DataVencimento ?? DateTime.MaxValue)
            .ThenByDescending(t => t.CriadoEm)
            .ToListAsync(ct);
    }

    public async Task AtualizarAsync(Tarefa tarefa, CancellationToken ct = default)
    {
        _context.Tarefas.Update(tarefa);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoverAsync(Tarefa tarefa, CancellationToken ct = default)
    {
        _context.Tarefas.Remove(tarefa);
        await _context.SaveChangesAsync(ct);
    }
}