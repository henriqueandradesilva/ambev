using Ambev.DeveloperEvaluation.Application.Sales.Commands.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Handlers.SaleHandlers.TestData;

public class SaleHandlerTest
{
    private readonly Faker _faker;

    public SaleHandlerTest()
    {
        _faker = new Faker();
    }

    public Sale GenerateRandomSale(
        Guid? saleId = null,
        List<SaleItem> saleItems = null)
    {
        return new Faker<Sale>()
        .RuleFor(s => s.Id, saleId ?? _faker.Random.Guid())
        .RuleFor(s => s.Customer, _faker.Name.FullName())
            .RuleFor(s => s.Branch, _faker.Company.CompanyName())
            .RuleFor(s => s.TotalAmount, _faker.Finance.Amount())
            .RuleFor(s => s.Date, _faker.Date.Past())
            .RuleFor(s => s.IsCancelled, false)
            .RuleFor(s => s.ListSaleItems, saleItems ?? GenerateRandomSaleItems(3))
            .Generate();
    }
    public List<SaleItem> GenerateRandomSaleItems(
        int count)
    {
        return new Faker<SaleItem>()
        .RuleFor(si => si.SaleId, _faker.Random.Guid())
            .RuleFor(si => si.Product, _faker.Commerce.ProductName())
            .RuleFor(si => si.Quantity, _faker.Random.Int(1, 5))
            .RuleFor(si => si.UnitPrice, _faker.Finance.Amount(10, 100))
            .RuleFor(si => si.TotalAmount, (f, si) => si.Quantity * si.UnitPrice - si.Discount)
            .Generate(count);
    }

    public GetSaleResult GenerateRandomGetSaleResult(
        Guid saleId)
    {
        return new Faker<GetSaleResult>()
            .RuleFor(r => r.Id, saleId)
            .RuleFor(r => r.TotalAmount, _faker.Finance.Amount(100, 1000))
            .Generate();
    }

    public static CreateSaleCommand GenerateValidCommand()
    {
        var faker = new Faker();
        var command = new CreateSaleCommand(
            faker.Person.FullName,
            faker.Company.CompanyName(),
            faker.Random.Guid().ToString(),
            DateTime.UtcNow);

        command.ListSaleItems = new Faker<CreateSaleItemCommand>()
            .RuleFor(i => i.Product, f => f.Commerce.ProductName())
            .RuleFor(i => i.Quantity, f => f.Random.Int(1, 20))
            .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(10, 500))
            .Generate(5);

        return command;
    }

    public static Sale GenerateFakeSale(
        CreateSaleCommand command)
    {
        return new Sale
        {
            Id = Guid.NewGuid(),
            Customer = command.Customer,
            Branch = command.Branch,
            Date = command.Date,
            ListSaleItems = command.ListSaleItems.Select(item => new SaleItem
            {
                Product = item.Product,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };
    }

    public static UpdateSaleCommand GenerateValidUpdateCommand()
    {
        return new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            Customer = "Updated Customer",
            Branch = "Updated Branch",
            Date = DateTime.UtcNow,
            IsCancelled = false
        };
    }

    public static Sale GenerateFakeSale(
        UpdateSaleCommand command)
    {
        return new Sale
        {
            Id = command.Id,
            Customer = "Original Customer",
            Branch = "Original Branch",
            Date = DateTime.UtcNow.AddDays(-1),
            ListSaleItems = new()
            {
                new SaleItem { Product = "Product A", Quantity = 2, UnitPrice = 50 },
                new SaleItem { Product = "Product B", Quantity = 3, UnitPrice = 30 }
            }
        };
    }
}