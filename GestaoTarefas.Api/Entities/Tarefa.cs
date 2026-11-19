using GestaoTarefas.Api.Enums;

namespace GestaoTarefas.Api.Entities;

public class Tarefa
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime? DataVencimento { get; set; }
    public StatusTarefa Status { get; set; } = StatusTarefa.Pendente;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}