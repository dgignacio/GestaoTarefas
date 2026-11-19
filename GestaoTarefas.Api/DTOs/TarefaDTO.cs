using System.ComponentModel.DataAnnotations;
using GestaoTarefas.Api.Enums;

namespace GestaoTarefas.Api.DTOs;

public record CriarTarefaRequest(
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(120, MinimumLength = 2, ErrorMessage = "O título deve ter entre 2 e 120 caracteres.")]
    string Titulo,

    [StringLength(1000, ErrorMessage = "A descrição deve ter no máximo 1000 caracteres.")]
    string? Descricao,

    DateTime? DataVencimento,

    [Required(ErrorMessage = "O status é obrigatório.")]
    StatusTarefa Status
);

public record AtualizarTarefaRequest(
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(120, MinimumLength = 2, ErrorMessage = "O título deve ter entre 2 e 120 caracteres.")]
    string Titulo,

    [StringLength(1000, ErrorMessage = "A descrição deve ter no máximo 1000 caracteres.")]
    string? Descricao,

    DateTime? DataVencimento,

    [Required(ErrorMessage = "O status é obrigatório.")]
    StatusTarefa Status
);

public record TarefaResponse(
    Guid Id,
    string Titulo,
    string? Descricao,
    DateTime? DataVencimento,
    string Status,
    DateTime CriadoEm
);