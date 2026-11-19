using FluentAssertions;
using GestaoTarefas.Api.Data;
using GestaoTarefas.Api.DTOs;
using GestaoTarefas.Api.Enums;
using GestaoTarefas.Api.Repositories;
using GestaoTarefas.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace GestaoTarefas.Tests;

public class TarefaServiceTests
{
    private static TarefaService CriarService()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);
        var repository = new TarefaRepository(context);
        return new TarefaService(repository, NullLogger<TarefaService>.Instance);
    }

    [Fact]
    public async Task CriarAsync_CriarTarefaERetornarCodigoUnico()
    {
        var service = CriarService();
        var request = new CriarTarefaRequest("Estudar Ingles", "Abrir Duolingo", DateTime.Today.AddDays(3), StatusTarefa.Pendente);

        var resultado = await service.CriarAsync(request);

        resultado.Id.Should().NotBeEmpty();
        resultado.Titulo.Should().Be("Estudar Ingles");
        resultado.Descricao.Should().Be("Abrir Duolingo");
        resultado.Status.Should().Be("Pendente");
    }

    [Fact]
    public async Task CriarAsync_LancarExcecaoQuandoTituloForApenasEspacos()
    {
        var service = CriarService();
        var request = new CriarTarefaRequest("   ", null, null, StatusTarefa.Pendente);

        Func<Task> acao = async () => await service.CriarAsync(request);

        await acao.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*título*");
    }

    [Fact]
    public async Task ListarAsync_FiltrarPorStatus()
    {
        var service = CriarService();
        await service.CriarAsync(new CriarTarefaRequest("Tarefa 1", null, null, StatusTarefa.Pendente));
        await service.CriarAsync(new CriarTarefaRequest("Tarefa 2", null, null, StatusTarefa.Concluida));

        var resultado = (await service.ListarAsync(status: StatusTarefa.Concluida)).ToList();

        resultado.Should().HaveCount(1);
        resultado[0].Titulo.Should().Be("Tarefa 2");
    }

    [Fact]
    public async Task ListarAsync_FiltrarPorDataVencimentoETermoDeBusca()
    {
        var service = CriarService();
        var dataAlvo = new DateTime(2026, 12, 10);

        await service.CriarAsync(new CriarTarefaRequest("Entregar API", "Projeto final", dataAlvo, StatusTarefa.EmProgresso));
        await service.CriarAsync(new CriarTarefaRequest("Reunião", "Alinhamento", dataAlvo.AddDays(1), StatusTarefa.EmProgresso));

        var resultado = (await service.ListarAsync(dataVencimento: dataAlvo, busca: "final")).ToList();

        resultado.Should().HaveCount(1);
        resultado[0].Titulo.Should().Be("Entregar API");
    }

    [Fact]
    public async Task AtualizarAsync_ModificarDadosQuandoTarefaExistir()
    {
        var service = CriarService();
        var criada = await service.CriarAsync(new CriarTarefaRequest("Título Antigo", "Desc antiga", null, StatusTarefa.Pendente));

        var updateRequest = new AtualizarTarefaRequest("Título Novo", "Desc nova", DateTime.Today.AddDays(5), StatusTarefa.Concluida);
        var atualizada = await service.AtualizarAsync(criada.Id, updateRequest);

        atualizada.Should().NotBeNull();
        atualizada!.Titulo.Should().Be("Título Novo");
        atualizada.Descricao.Should().Be("Desc nova");
        atualizada.Status.Should().Be("Concluida");
    }

    [Fact]
    public async Task AtualizarAsync_RetornarNullQuandoTarefaNaoExistir()
    {
        var service = CriarService();
        var updateRequest = new AtualizarTarefaRequest("Título", null, null, StatusTarefa.Pendente);

        var atualizada = await service.AtualizarAsync(Guid.NewGuid(), updateRequest);

        atualizada.Should().BeNull();
    }

    [Fact]
    public async Task ExcluirAsync_RemoverTarefaExistenteERetornarTrue()
    {
        var service = CriarService();
        var criada = await service.CriarAsync(new CriarTarefaRequest("Tarefa para excluir", null, null, StatusTarefa.Pendente));

        var excluida = await service.ExcluirAsync(criada.Id);
        var buscaAposExclusao = await service.ObterPorIdAsync(criada.Id);

        excluida.Should().BeTrue();
        buscaAposExclusao.Should().BeNull();
    }

    [Fact]
    public async Task ExcluirAsync_RetornarFalseQuandoTarefaNaoExistir()
    {
        var service = CriarService();

        var excluida = await service.ExcluirAsync(Guid.NewGuid());

        excluida.Should().BeFalse();
    }
}