using Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Validation.TestData;

public class SaleValidationTest
{
    private readonly Faker _faker;

    public SaleValidationTest()
    {
        _faker = new Faker();
    }

    public Sale GenerateRandomSale(
        Guid? saleId = null,
        List<SaleItem> saleItems = null)
    {
        var sale = new Faker<Sale>()
        .RuleFor(s => s.Id, saleId ?? _faker.Random.Guid())
        .RuleFor(s => s.Customer, _faker.Name.FullName())
            .RuleFor(s => s.Branch, _faker.Company.CompanyName())
            .RuleFor(s => s.TotalAmount, _faker.Finance.Amount())
            .RuleFor(s => s.Date, _faker.Date.Past())
            .RuleFor(s => s.IsCancelled, false)
            .RuleFor(s => s.ListSaleItems, saleItems ?? GenerateRandomSaleItems(3))
            .Generate();

        sale.CalculateTotalAmount();

        return sale;
    }
    public List<SaleItem> GenerateRandomSaleItems(
        int count)
    {
        var listSaleItem = new Faker<SaleItem>()
        .RuleFor(si => si.SaleId, _faker.Random.Guid())
            .RuleFor(si => si.Product, _faker.Commerce.ProductName())
            .RuleFor(si => si.Quantity, _faker.Random.Int(1, 5))
            .RuleFor(si => si.UnitPrice, _faker.Finance.Amount(10, 100))
            .RuleFor(si => si.Discount, 0)
            .RuleFor(si => si.TotalAmount, (f, si) => si.Quantity * si.UnitPrice - si.Discount)
            .Generate(count);

        listSaleItem.ForEach(item => item.ApplyDiscount());

        return listSaleItem;
    }

    public GetSaleResult GenerateRandomGetSaleResult(
        Guid saleId)
    {
        return new Faker<GetSaleResult>()
            .RuleFor(r => r.Id, saleId)
            .RuleFor(r => r.TotalAmount, _faker.Finance.Amount(100, 1000))
            .Generate();
    }
}