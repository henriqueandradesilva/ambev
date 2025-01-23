using Ambev.DeveloperEvaluation.Application.Sales.Commands.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.Events.Handlers.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Handlers.SaleHandlers.TestData;
using Bogus;
using FluentAssertions;
using NSubstitute;
using System.Linq.Expressions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Handlers.SaleHandlers;

public class CancelSaleItemHandlerTests : SaleHandlerTest
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleItemCancelledEventHandler _saleItemCancelledEventHandler;
    private readonly CancelSaleItemHandler _handler;
    private readonly Faker _faker;

    public CancelSaleItemHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _saleItemCancelledEventHandler = Substitute.For<ISaleItemCancelledEventHandler>();
        _handler = new CancelSaleItemHandler(_saleRepository, _saleItemCancelledEventHandler);
        _faker = new Faker();
    }

    [Fact(DisplayName = "Should throw KeyNotFoundException when sale does not exist")]
    public async Task Handle_SaleDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var command = new CancelSaleItemCommand(_faker.Random.Guid(), _faker.Random.Guid());

        _saleRepository.GetByIdWithIncludeAsync(command.SaleId, Arg.Any<CancellationToken>(), Arg.Any<Expression<Func<Sale, object>>>())
            .Returns(Task.FromResult<Sale>(null));

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.SaleId} not found.");
    }

    [Fact(DisplayName = "Should throw InvalidOperationException when sale is already cancelled")]
    public async Task Handle_SaleIsAlreadyCancelled_ThrowsInvalidOperationException()
    {
        // Arrange
        var command = new CancelSaleItemCommand(_faker.Random.Guid(), _faker.Random.Guid());

        var sale = GenerateRandomSale(command.SaleId);  // Generates a random sale
        sale.CancelSale(true);  // Simulating that the sale is already cancelled

        _saleRepository.GetByIdWithIncludeAsync(command.SaleId, Arg.Any<CancellationToken>(), Arg.Any<Expression<Func<Sale, object>>>())
            .Returns(Task.FromResult(sale));

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Sale is already cancelled.");
    }

    [Fact(DisplayName = "Should throw KeyNotFoundException when sale item does not exist")]
    public async Task Handle_SaleItemDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var command = new CancelSaleItemCommand(_faker.Random.Guid(), _faker.Random.Guid());

        var sale = GenerateRandomSale(command.SaleId, new List<SaleItem>()); // No sale items

        _saleRepository.GetByIdWithIncludeAsync(command.SaleId, Arg.Any<CancellationToken>(), Arg.Any<Expression<Func<Sale, object>>>())
            .Returns(Task.FromResult(sale));

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale Item with ID {command.Id} not found in sale.");
    }

    [Fact(DisplayName = "Should throw InvalidOperationException when sale item is already cancelled")]
    public async Task Handle_SaleItemIsAlreadyCancelled_ThrowsInvalidOperationException()
    {
        // Arrange
        var command = new CancelSaleItemCommand(_faker.Random.Guid(), _faker.Random.Guid());

        var saleItem = new SaleItem
        {
            Id = command.Id
        };

        saleItem.Cancel();  // Mark item as cancelled

        var sale = GenerateRandomSale(command.SaleId, new List<SaleItem> { saleItem });

        _saleRepository.GetByIdWithIncludeAsync(command.SaleId, Arg.Any<CancellationToken>(), Arg.Any<Expression<Func<Sale, object>>>())
            .Returns(Task.FromResult(sale));

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Sale item is already cancelled.");
    }

    [Fact(DisplayName = "Should cancel the sale item and publish the SaleItemCancelledEvent when valid sale and item are provided")]
    public async Task Handle_ValidSaleAndSaleItem_CancelsItemAndPublishesEvent()
    {
        // Arrange
        var command = new CancelSaleItemCommand(_faker.Random.Guid(), _faker.Random.Guid());

        var saleItem = new SaleItem
        {
            Id = command.Id
        };

        var sale = GenerateRandomSale(command.SaleId, new List<SaleItem> { saleItem });

        _saleRepository.GetByIdWithIncludeAsync(command.SaleId, Arg.Any<CancellationToken>(), Arg.Any<Expression<Func<Sale, object>>>())
            .Returns(Task.FromResult(sale));

        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(x => Task.FromResult(x.ArgAt<Sale>(0)));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("The sale item was successfully cancelled.");

        _saleRepository.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
        _saleItemCancelledEventHandler.Received(1).Handle(Arg.Is<SaleItemCancelledEvent>(e => e.Sale.Id == command.SaleId && e.SaleItem.Id == command.Id), Arg.Any<CancellationToken>());
    }
}
