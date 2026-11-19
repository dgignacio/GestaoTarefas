using GestaoTarefas.Api.DTOs;
using GestaoTarefas.Api.Enums;

namespace GestaoTarefas.Api.Services;

public interface ITarefaService
{
    Task<TarefaResponse> CriarAsync(CriarTarefaRequest request, CancellationToken ct = default);
    Task<TarefaResponse?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<TarefaResponse>> ListarAsync(
        StatusTarefa? status = null,
        DateTime? dataVencimento = null,
        string? busca = null,
        CancellationToken ct = default);
    Task<TarefaResponse?> AtualizarAsync(Guid id, AtualizarTarefaRequest request, CancellationToken ct = default);
    Task<bool> ExcluirAsync(Guid id, CancellationToken ct = default);
}