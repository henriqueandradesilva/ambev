using Ambev.DeveloperEvaluation.Application.Sales.Commands.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Handlers.SaleHandlers.TestData;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Handlers.SaleHandlers;

public class DeleteSaleHandlerTests : SaleHandlerTest
{
    private readonly ISaleRepository _saleRepository;
    private readonly DeleteSaleHandler _handler;

    public DeleteSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _handler = new DeleteSaleHandler(_saleRepository);
    }

    [Fact(DisplayName = "Should return validation error when ID is empty")]
    public async Task Validate_EmptyId_ShouldReturnErrorMessage()
    {
        // Arrange
        var command = new DeleteSaleCommand(Guid.Empty);

        var validator = new DeleteSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, CancellationToken.None);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.PropertyName == "Id" && e.ErrorMessage == "The sale ID must be provided.");
    }

    [Fact(DisplayName = "Should throw KeyNotFoundException when sale is not found")]
    public async Task Handle_SaleNotFound_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var command = new DeleteSaleCommand(GenerateRandomSale().Id);

        _saleRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(false));

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.Id} not found");
    }

    [Fact(DisplayName = "Should successfully delete sale when request is valid")]
    public async Task Handle_ValidRequest_ShouldReturnSuccessAndDeleteSale()
    {
        // Arrange
        var command = new DeleteSaleCommand(GenerateRandomSale().Id);

        _saleRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("The sale was successfully deleted.");

        _saleRepository.Received(1).DeleteAsync(command.Id, Arg.Any<CancellationToken>());
    }
}
