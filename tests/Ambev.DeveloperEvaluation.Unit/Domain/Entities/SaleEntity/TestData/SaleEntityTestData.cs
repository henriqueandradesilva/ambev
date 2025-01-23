using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.SaleEntity.TestData;

public class SaleEntityTestData
{
    private readonly Faker _faker;

    public SaleEntityTestData()
    {
        _faker = new Faker();
    }

    public Sale GenerateValidSale()
    {
        return new Faker<Sale>()
            .RuleFor(s => s.Customer, _faker.Name.FullName())
            .RuleFor(s => s.Branch, _faker.Company.CompanyName())
            .RuleFor(s => s.Date, _faker.Date.Past())
            .Generate();
    }

    public SaleItem GenerateSaleItem()
    {
        return new Faker<SaleItem>()
            .RuleFor(si => si.Product, _faker.Commerce.ProductName())
            .RuleFor(si => si.Quantity, _faker.Random.Int(1, 5))
            .RuleFor(si => si.UnitPrice, _faker.Finance.Amount(10, 100))
            .RuleFor(si => si.Discount, _faker.Finance.Amount(0, 30))
            .RuleFor(si => si.TotalAmount, (f, si) => si.Quantity * si.UnitPrice - si.Discount)
            .Generate();
    }
}