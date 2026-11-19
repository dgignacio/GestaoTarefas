using GestaoTarefas.Api.Entities;
using GestaoTarefas.Api.Enums;

namespace GestaoTarefas.Api.Repositories;

public interface ITarefaRepository
{
    Task<Tarefa> AdicionarAsync(Tarefa tarefa, CancellationToken ct = default);
    Task<Tarefa?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Tarefa>> ListarAsync(
        StatusTarefa? status = null,
        DateTime? dataVencimento = null,
        string? busca = null,
        CancellationToken ct = default);
    Task AtualizarAsync(Tarefa tarefa, CancellationToken ct = default);
    Task RemoverAsync(Tarefa tarefa, CancellationToken ct = default);
}