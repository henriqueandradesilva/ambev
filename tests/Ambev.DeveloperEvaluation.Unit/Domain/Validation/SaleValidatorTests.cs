using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Unit.Domain.Validation.TestData;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Validation;

public class SaleValidatorTests : SaleValidationTest
{
    private readonly SaleValidator _validator;
    private readonly Faker _faker;

    public SaleValidatorTests()
    {
        _validator = new SaleValidator();
        _faker = new Faker();
    }

    [Fact(DisplayName = "Validate should pass when all fields are valid")]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        // Arrange
        var sale = GenerateRandomSale();

        // Act
        var result = _validator.Validate(sale);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Validate should fail when customer name is empty")]
    public void Validate_ShouldFail_WhenCustomerIsEmpty()
    {
        // Arrange
        var sale = GenerateRandomSale();
        sale.Customer = string.Empty;

        // Act
        var result = _validator.Validate(sale);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.ErrorMessage == "The customer name must be specified.");
    }

    [Fact(DisplayName = "Validate should fail when customer name exceeds max length")]
    public void Validate_ShouldFail_WhenCustomerExceedsMaxLength()
    {
        // Arrange
        var sale = GenerateRandomSale();
        sale.Customer = new string('C', 151); // Exceeds 150 characters

        // Act
        var result = _validator.Validate(sale);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.ErrorMessage == "The customer name must not exceed 150 characters.");
    }

    [Fact(DisplayName = "Validate should fail when branch name is empty")]
    public void Validate_ShouldFail_WhenBranchIsEmpty()
    {
        // Arrange
        var sale = GenerateRandomSale();
        sale.Branch = string.Empty;

        // Act
        var result = _validator.Validate(sale);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.ErrorMessage == "The branch must be specified.");
    }

    [Fact(DisplayName = "Validate should fail when branch name exceeds max length")]
    public void Validate_ShouldFail_WhenBranchExceedsMaxLength()
    {
        // Arrange
        var sale = GenerateRandomSale();
        sale.Branch = new string('B', 151); // Exceeds 150 characters

        // Act
        var result = _validator.Validate(sale);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.ErrorMessage == "The branch name must not exceed 150 characters.");
    }

    [Fact(DisplayName = "Validate should fail when no sale items are provided")]
    public void Validate_ShouldFail_WhenNoSaleItemsProvided()
    {
        // Arrange
        var sale = GenerateRandomSale();
        sale.ListSaleItems = new List<SaleItem>(); // Empty list

        // Act
        var result = _validator.Validate(sale);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.ErrorMessage == "At least one item must be included in the sale.");
    }
}
