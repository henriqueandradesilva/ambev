using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Ambev.DeveloperEvaluation.Unit.Utils;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.ORM.Repositories;

public class SaleRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly DbContextOptions<DefaultContext> _dbContextOptions;

    public SaleRepositoryTests(
        DatabaseFixture fixture)
    {
        _fixture = fixture;

        _dbContextOptions = new DbContextOptionsBuilder<DefaultContext>()
            .UseNpgsql(_fixture.ConnectionString)
            .Options;
    }

    [Fact(DisplayName = "Given predicate When finding sales Then it should return matching sales")]
    public async Task FindSalesByPredicate_ShouldReturnMatchingSales()
    {
        // Arrange
        await using var context = new DefaultContext(_dbContextOptions);

        var saleRepository = new SaleRepository(context);

        var sale1 = new Sale
        {
            Id = Guid.NewGuid(),
            Customer = "Customer D",
            Branch = "Branch E",
            Number = "S12348",
            Date = DateTime.UtcNow,
            ListSaleItems = new List<SaleItem>
            {
                new SaleItem { Product = "Product 5", Quantity = 6, UnitPrice = 250 }
            }
        };

        var sale2 = new Sale
        {
            Id = Guid.NewGuid(),
            Customer = "Customer E",
            Branch = "Branch F",
            Number = "S12349",
            Date = DateTime.UtcNow,
            ListSaleItems = new List<SaleItem>
            {
                new SaleItem { Product = "Product 6", Quantity = 7, UnitPrice = 300 }
            }
        };

        await saleRepository.CreateAsync(sale1);
        await saleRepository.CreateAsync(sale2);
        await context.SaveChangesAsync();

        // Act
        var foundSales = await saleRepository.FindListAsync(x => x.Customer.Contains("Customer D"), default);

        // Assert
        foundSales.Should().HaveCount(1);
        foundSales.First().Customer.Should().Be("Customer D");
    }

    [Fact(DisplayName = "Given pagination parameters When getting sales Then it should return paginated list")]
    public async Task GetSalesWithPagination_ShouldReturnPaginatedList()
    {
        // Arrange
        await using var context = new DefaultContext(_dbContextOptions);

        var saleRepository = new SaleRepository(context);

        // Criando 50 vendas com clientes de Customer 0 até Customer 49
        for (int i = 0; i < 50; i++)
        {
            var sale = new Sale
            {
                Id = Guid.NewGuid(),
                Customer = $"Customer {i}",
                Branch = "Branch G",
                Number = $"S1234{i}",
                Date = DateTime.UtcNow,
                ListSaleItems = new List<SaleItem>
            {
                new SaleItem { Product = $"Product {i}", Quantity = 1, UnitPrice = 100 }
            }
            };
            await saleRepository.CreateAsync(sale);
        }

        await context.SaveChangesAsync();

        // Act
        var pageNumber = 1;
        var pageSize = 10;

        // Passando uma query customizada para garantir a ordenação por Customer
        var paginatedSales = await saleRepository.GetListSaleWithPaginationAsync(
            pageNumber,
            pageSize,
            queryCustomizer: query => query.OrderBy(sale => sale.Customer)
        );

        // Assert
        paginatedSales.Should().HaveCount(10);  // Esperamos 10 itens
        paginatedSales.First().Customer.Should().Be("Customer 0");  // Verifique o primeiro item da página
        paginatedSales.Last().Customer.Should().Be("Customer 17");   // Verifique o último item da página
    }

    [Fact(DisplayName = "Given valid sale When create Then it should create")]
    public async Task AddSale_ShouldCreateInDatabase()
    {
        // Arrange
        await using var context = new DefaultContext(_dbContextOptions);

        var saleRepository = new SaleRepository(context);

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            Customer = "Customer A",
            Branch = "Branch B",
            Number = "S12345",
            Date = DateTime.UtcNow,
            ListSaleItems = new List<SaleItem>
            {
                new SaleItem { Product = "Product 1", Quantity = 2, UnitPrice = 100 },
                new SaleItem { Product = "Product 2", Quantity = 1, UnitPrice = 50 }
            }
        };

        // Act
        await saleRepository.CreateAsync(sale);
        await context.SaveChangesAsync();

        // Assert
        var savedSale = await saleRepository.GetByIdWithIncludeAsync(
            sale.Id,
            default,
            x => x.ListSaleItems);

        savedSale.Should().NotBeNull();
        savedSale.Id.Should().Be(sale.Id);
        savedSale.Customer.Should().Be("Customer A");
        savedSale.ListSaleItems.Should().HaveCount(2);
    }

    [Fact(DisplayName = "Given sale ID When updating Then it should update the sale")]
    public async Task UpdateSale_ShouldUpdateInDatabase()
    {
        // Arrange
        await using var context = new DefaultContext(_dbContextOptions);

        var saleRepository = new SaleRepository(context);

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            Customer = "Customer C",
            Branch = "Branch D",
            Number = "S12347",
            Date = DateTime.UtcNow,
            ListSaleItems = new List<SaleItem>
            {
                new SaleItem { Product = "Product 4", Quantity = 4, UnitPrice = 200 }
            }
        };

        await saleRepository.CreateAsync(sale);
        await context.SaveChangesAsync();

        sale.Customer = "Updated Customer C";
        sale.ListSaleItems[0].Quantity = 5;

        // Act
        await saleRepository.UpdateAsync(sale);
        await context.SaveChangesAsync();

        // Assert
        var updatedSale = await saleRepository.GetByIdAsync(sale.Id, default);
        updatedSale.Should().NotBeNull();
        updatedSale.Customer.Should().Be("Updated Customer C");
        updatedSale.ListSaleItems[0].Quantity.Should().Be(5);
    }

    [Fact(DisplayName = "Given sale ID When deleting Then it should remove the sale")]
    public async Task DeleteSale_ShouldRemoveFromDatabase()
    {
        // Arrange
        await using var context = new DefaultContext(_dbContextOptions);

        var saleRepository = new SaleRepository(context);

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            Customer = "Customer B",
            Branch = "Branch C",
            Number = "S12346",
            Date = DateTime.UtcNow,
            ListSaleItems = new List<SaleItem>
            {
                new SaleItem { Product = "Product 3", Quantity = 3, UnitPrice = 150 }
            }
        };

        await saleRepository.CreateAsync(sale);
        await context.SaveChangesAsync();

        // Act
        var result = await saleRepository.DeleteAsync(sale.Id);
        await context.SaveChangesAsync();

        // Assert
        var deletedSale = await saleRepository.GetByIdAsync(sale.Id, default);
        deletedSale.Should().BeNull();
        result.Should().BeTrue();
    }
}