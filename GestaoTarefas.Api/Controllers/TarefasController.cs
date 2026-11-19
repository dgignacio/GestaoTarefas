using GestaoTarefas.Api.DTOs;
using GestaoTarefas.Api.Enums;
using GestaoTarefas.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestaoTarefas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TarefasController : ControllerBase
{
    private readonly ITarefaService _service;

    public TarefasController(ITarefaService service) => _service = service;

    /// <summary>
    /// Cadastra uma nova tarefa e retorna seu código (ID).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TarefaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TarefaResponse>> Criar([FromBody] CriarTarefaRequest request, CancellationToken ct)
    {
        var criada = await _service.CriarAsync(request, ct);
        return CreatedAtAction(nameof(ObterPorId), new { id = criada.Id }, criada);
    }

    /// <summary>
    /// Lista todas as tarefas, permitindo filtrar por status, data de vencimento e texto de busca.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TarefaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TarefaResponse>>> Listar(
        [FromQuery] StatusTarefa? status,
        [FromQuery] DateTime? dataVencimento,
        [FromQuery] string? busca,
        CancellationToken ct)
    {
        var tarefas = await _service.ListarAsync(status, dataVencimento, busca, ct);
        return Ok(tarefas);
    }

    /// <summary>
    /// Busca uma tarefa específica pelo seu código (ID).
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TarefaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TarefaResponse>> ObterPorId([FromRoute] Guid id, CancellationToken ct)
    {
        var tarefa = await _service.ObterPorIdAsync(id, ct);
        return tarefa is null ? NotFound(new { mensagem = "Tarefa não encontrada." }) : Ok(tarefa);
    }

    /// <summary>
    /// Atualiza título, descrição, data de vencimento e status de uma tarefa existente.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TarefaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TarefaResponse>> Atualizar(
        [FromRoute] Guid id,
        [FromBody] AtualizarTarefaRequest request,
        CancellationToken ct)
    {
        var atualizada = await _service.AtualizarAsync(id, request, ct);
        return atualizada is null ? NotFound(new { mensagem = "Tarefa não encontrada." }) : Ok(atualizada);
    }

    /// <summary>
    /// Exclui uma tarefa pelo seu código (ID).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir([FromRoute] Guid id, CancellationToken ct)
    {
        var excluida = await _service.ExcluirAsync(id, ct);
        return excluida ? NoContent() : NotFound(new { mensagem = "Tarefa não encontrada." });
    }
}