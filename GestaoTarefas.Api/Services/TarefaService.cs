using GestaoTarefas.Api.DTOs;
using GestaoTarefas.Api.Entities;
using GestaoTarefas.Api.Enums;
using GestaoTarefas.Api.Repositories;

namespace GestaoTarefas.Api.Services;

public class TarefaService : ITarefaService
{
    private readonly ITarefaRepository _repository;
    private readonly ILogger<TarefaService> _logger;

    public TarefaService(ITarefaRepository repository, ILogger<TarefaService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<TarefaResponse> CriarAsync(CriarTarefaRequest request, CancellationToken ct = default)
    {
        ValidarRegrasNegocio(request.Titulo, request.Status);

        var tarefa = new Tarefa
        {
            Titulo = request.Titulo.Trim(),
            Descricao = string.IsNullOrWhiteSpace(request.Descricao) ? null : request.Descricao.Trim(),
            DataVencimento = request.DataVencimento,
            Status = request.Status
        };

        var criada = await _repository.AdicionarAsync(tarefa, ct);
        _logger.LogInformation("Tarefa criada com sucesso. Id: {TarefaId}, Titulo: {Titulo}", criada.Id, criada.Titulo);

        return MapearParaResponse(criada);
    }

    public async Task<TarefaResponse?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
    {
        var tarefa = await _repository.ObterPorIdAsync(id, ct);
        return tarefa is null ? null : MapearParaResponse(tarefa);
    }

    public async Task<IEnumerable<TarefaResponse>> ListarAsync(
        StatusTarefa? status = null,
        DateTime? dataVencimento = null,
        string? busca = null,
        CancellationToken ct = default)
    {
        var tarefas = await _repository.ListarAsync(status, dataVencimento, busca, ct);
        return tarefas.Select(MapearParaResponse);
    }

    public async Task<TarefaResponse?> AtualizarAsync(Guid id, AtualizarTarefaRequest request, CancellationToken ct = default)
    {
        ValidarRegrasNegocio(request.Titulo, request.Status);

        var tarefa = await _repository.ObterPorIdAsync(id, ct);
        if (tarefa is null)
        {
            _logger.LogWarning("Tentativa de atualizar tarefa inexistente. Id: {TarefaId}", id);
            return null;
        }

        tarefa.Titulo = request.Titulo.Trim();
        tarefa.Descricao = string.IsNullOrWhiteSpace(request.Descricao) ? null : request.Descricao.Trim();
        tarefa.DataVencimento = request.DataVencimento;
        tarefa.Status = request.Status;

        await _repository.AtualizarAsync(tarefa, ct);
        _logger.LogInformation("Tarefa atualizada com sucesso. Id: {TarefaId}", tarefa.Id);

        return MapearParaResponse(tarefa);
    }

    public async Task<bool> ExcluirAsync(Guid id, CancellationToken ct = default)
    {
        var tarefa = await _repository.ObterPorIdAsync(id, ct);
        if (tarefa is null)
        {
            _logger.LogWarning("Tentativa de excluir tarefa inexistente. Id: {TarefaId}", id);
            return false;
        }

        await _repository.RemoverAsync(tarefa, ct);
        _logger.LogInformation("Tarefa excluída com sucesso. Id: {TarefaId}", id);
        return true;
    }

    private static void ValidarRegrasNegocio(string titulo, StatusTarefa status)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("O título da tarefa não pode ser vazio ou conter apenas espaços.");

        if (!Enum.IsDefined(typeof(StatusTarefa), status))
            throw new ArgumentException("Status inválido. Valores aceitos: Pendente, EmProgresso ou Concluida.");
    }

    private static TarefaResponse MapearParaResponse(Tarefa t) => new(
        t.Id,
        t.Titulo,
        t.Descricao,
        t.DataVencimento,
        t.Status.ToString(),
        t.CriadoEm
    );
}