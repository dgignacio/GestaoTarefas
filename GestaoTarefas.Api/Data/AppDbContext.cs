using GestaoTarefas.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestaoTarefas.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Tarefa> Tarefas => Set<Tarefa>();
}