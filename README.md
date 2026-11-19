# API de Gestão de Tarefas (.NET)

API RESTful desenvolvida em **ASP.NET Core (.NET 11)** para gestão de tarefas (CRUD completo, filtragem por status e data de vencimento, e busca textual), utilizando **Entity Framework Core InMemory**, **Swagger**, princípios **SOLID** e testes automatizados com **xUnit**.

---

## Arquitetura e Decisões Técnicas

O projeto segue uma organização em camadas para separar responsabilidades e facilitar manutenção e testes:

- **Controllers (`TarefasController`):** Camada HTTP responsável por rotas, leitura de parâmetros/payloads e retorno dos códigos de status RESTful (`200`, `201`, `204`, `400`, `404`).
- **Services (`TarefaService`):** Camada de regras de negócio, validações, higienização de entradas, logging e mapeamento entre Entidades e DTOs.
- **Repositories (`TarefaRepository`):** Camada de acesso a dados que encapsula o Entity Framework Core InMemory e aplica os filtros de consulta.
- **DTOs (`CriarTarefaRequest`, `AtualizarTarefaRequest`, `TarefaResponse`):** Contratos de entrada e saída com validações via Data Annotations, isolando o modelo de domínio.
- **Middleware (`ExceptionHandlingMiddleware`):** Tratamento centralizado de exceções, garantindo respostas de erro padronizadas em JSON.

### Motivo da escolha desse modelo
1. **Separação clara de responsabilidades:** Cada camada tem um papel bem definido, com interfaces (`ITarefaService`, `ITarefaRepository`) injetadas via construtor.
2. **Testabilidade:** Permite testar regras de negócio com xUnit e banco InMemory de forma isolada.
3. **Evolução simples:** Atende ao escopo do desafio sem complexidade desnecessária e facilita a troca do banco InMemory por um banco relacional no futuro.

---

## Como Rodar o Projeto

### Pré-requisitos
- [.NET 11 SDK](https://dotnet.microsoft.com/download) (ou .NET 10+)
- Visual Studio (ou VS Code / CLI)

### Pelo Visual Studio
1. Abra `GestaoTarefas.slnx`.
2. Defina `GestaoTarefas.Api` como projeto de inicialização (*Set as Startup Project*).
3. Pressione **F5** e acesse o **Swagger** em `/swagger`.

### Pelo Terminal
A partir da pasta raiz do repositório (`GestaoTarefas`), execute:

```bash
dotnet run --project GestaoTarefas.Api
```

---

## Como Rodar os Testes Automatizados

### Pelo Visual Studio
1. Vá em **Test > Test Explorer**.
2. Clique em **Run All Tests**.

### Pelo Terminal
A partir da pasta raiz do repositório (`GestaoTarefas`), execute:

```bash
dotnet test
```

*(Opcionalmente, você pode executar direto o projeto de testes com `dotnet test GestaoTarefas.Tests/GestaoTarefas.Tests.csproj`).*

---

## Endpoints da API

| Método | Rota | Descrição | Status HTTP |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/tarefas` | Cria uma nova tarefa e retorna seu ID | `201`, `400` |
| `GET` | `/api/tarefas` | Lista tarefas (filtros: `status`, `dataVencimento`, `busca`) | `200` |
| `GET` | `/api/tarefas/{id}` | Obtém uma tarefa pelo seu ID | `200`, `404` |
| `PUT` | `/api/tarefas/{id}` | Atualiza título, descrição, data e status da tarefa | `200`, `400`, `404` |
| `DELETE` | `/api/tarefas/{id}` | Remove uma tarefa pelo ID | `204`, `404` |

### Exemplo de Requisição (`POST /api/tarefas`)
```json
{
  "titulo": "Finalizar API de Tarefas",
  "descricao": "Validar e publicar no GitHub",
  "dataVencimento": "2026-12-01T00:00:00",
  "status": "Pendente"
}
```
*Status permitidos:* `Pendente`, `EmProgresso`, `Concluida`.