namespace Ambev.DeveloperEvaluation.Unit.Utils;

using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using Xunit;

public class DatabaseFixture : IAsyncLifetime
{
    public PostgreSqlContainer PostgreSqlContainer { get; private set; }
    public string ConnectionString => PostgreSqlContainer?.GetConnectionString();


    public ISaleRepository SaleRepository { get; private set; }
    public DefaultContext Context { get; private set; }

    public async Task InitializeAsync()
    {
        PostgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase($"testdb{Guid.NewGuid()}")
            .WithUsername("testuser")
            .WithPassword("testpassword")
            .Build();

        await PostgreSqlContainer.StartAsync();

        // Criar opções do DbContext para conexão com o banco
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        // Criar instância do DbContext
        Context = new DefaultContext(options);

        // Aplicar as migrations
        await Context.Database.MigrateAsync();

        // Inicializar repositórios
        SaleRepository = new SaleRepository(Context);
    }

    public async Task DisposeAsync()
    {
        // Fechar o container do PostgreSQL após os testes
        if (PostgreSqlContainer != null)
        {
            await PostgreSqlContainer.StopAsync();
            await PostgreSqlContainer.DisposeAsync();
        }
    }
}
