using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.SaleEntity.TestData;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.SaleEntity;

public class SaleTests : SaleEntityTestData
{
    private readonly Faker _faker;

    public SaleTests()
    {
        _faker = new Faker();
    }

    [Fact(DisplayName = "Constructor should initialize properties when sale is created")]
    public void Constructor_ShouldInitializeProperties_WhenSaleIsCreated()
    {
        // Arrange
        var sale = new Sale();

        // Act & Assert
        sale.Id.Should().NotBeEmpty();
        sale.Number.Should().NotBeEmpty();
        sale.IsCancelled.Should().BeFalse();
    }

    [Fact(DisplayName = "Validate should return valid when all fields are valid")]
    public void Validate_ShouldReturnValid_WhenAllFieldsAreValid()
    {
        // Arrange
        var sale = GenerateValidSale();

        var saleItem = GenerateSaleItem();

        sale.AddSaleItem(saleItem);

        // Act
        var validationResult = sale.Validate();

        // Assert
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = "Validate should return invalid when customer is empty")]
    public void Validate_ShouldReturnInvalid_WhenCustomerIsEmpty()
    {
        // Arrange
        var sale = GenerateValidSale();
        sale.Customer = string.Empty;

        // Act
        var validationResult = sale.Validate();

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().ContainSingle(e => e.Detail == "The customer name must be specified.");
    }

    [Fact(DisplayName = "AddSaleItem should update total amount when item is added")]
    public void AddSaleItem_ShouldUpdateTotalAmount_WhenItemIsAdded()
    {
        // Arrange
        var sale = GenerateValidSale();

        var saleItem = GenerateSaleItem();

        sale.AddSaleItem(saleItem);

        // Act
        sale.CalculateTotalAmount();

        // Assert
        sale.TotalAmount.Should().Be(saleItem.TotalAmount);
        sale.ListSaleItems.Should().Contain(saleItem);
    }

    [Fact(DisplayName = "CalculateTotalAmount should calculate total when items are added")]
    public void CalculateTotalAmount_ShouldCalculateTotal_WhenItemsAreAdded()
    {
        // Arrange
        var sale = GenerateValidSale();

        var saleItem1 = GenerateSaleItem();
        var saleItem2 = GenerateSaleItem();

        sale.AddSaleItem(saleItem1);
        sale.AddSaleItem(saleItem2);

        // Act
        sale.CalculateTotalAmount();

        // Assert
        sale.TotalAmount.Should().Be(saleItem1.TotalAmount + saleItem2.TotalAmount);
    }

    [Fact(DisplayName = "CancelSale should mark sale as cancelled when called with true")]
    public void CancelSale_ShouldMarkAsCancelled_WhenCalledWithTrue()
    {
        // Arrange
        var sale = GenerateValidSale();

        var saleItem1 = GenerateSaleItem();
        var saleItem2 = GenerateSaleItem();

        sale.AddSaleItem(saleItem1);
        sale.AddSaleItem(saleItem2);

        // Act
        sale.CancelSale(true);

        // Assert
        sale.IsCancelled.Should().BeTrue();
        sale.ListSaleItems.Should().OnlyContain(item => item.IsCancelled);
    }

    [Fact(DisplayName = "CancelSaleItem should cancel item and recalculate total when item is cancelled")]
    public void CancelSaleItem_ShouldCancelItemAndRecalculate_WhenItemIsCancelled()
    {
        // Arrange
        var sale = GenerateValidSale();
        var saleItem = GenerateSaleItem();
        sale.AddSaleItem(saleItem);

        // Act
        sale.CancelSaleItem(saleItem.Id);

        // Assert
        saleItem.IsCancelled.Should().BeTrue();
        sale.TotalAmount.Should().Be(0); // Assuming only the cancelled item exists
    }
}
