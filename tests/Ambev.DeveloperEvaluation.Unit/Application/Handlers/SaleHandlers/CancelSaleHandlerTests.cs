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

public class CancelSaleHandlerTests : SaleHandlerTest
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleCancelledEventHandler _saleCancelledEventHandler;
    private readonly CancelSaleHandler _handler;
    private readonly Faker _faker;

    public CancelSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _saleCancelledEventHandler = Substitute.For<ISaleCancelledEventHandler>();
        _handler = new CancelSaleHandler(_saleRepository, _saleCancelledEventHandler);
        _faker = new Faker();
    }

    [Fact(DisplayName = "Should throw KeyNotFoundException when sale does not exist")]
    public async Task Handle_SaleDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var command = new CancelSaleCommand(_faker.Random.Guid());

        _saleRepository.GetByIdWithIncludeAsync(command.Id, Arg.Any<CancellationToken>(), Arg.Any<Expression<Func<Sale, object>>>())
            .Returns(Task.FromResult<Sale>(null));

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.Id} not found.");
    }

    [Fact(DisplayName = "Should throw InvalidOperationException when sale is already cancelled")]
    public async Task Handle_SaleIsAlreadyCancelled_ThrowsInvalidOperationException()
    {
        // Arrange
        var command = new CancelSaleCommand(_faker.Random.Guid());

        var sale = GenerateRandomSale(command.Id);
        sale.CancelSale(true);

        _saleRepository.GetByIdWithIncludeAsync(command.Id, Arg.Any<CancellationToken>(), Arg.Any<Expression<Func<Sale, object>>>())
            .Returns(Task.FromResult(sale));

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Sale is already cancelled.");
    }

    [Fact(DisplayName = "Should cancel the sale and publish SaleCancelledEvent when valid sale data is provided")]
    public async Task Handle_ValidSaleData_CancelsSaleAndPublishesEvent()
    {
        // Arrange
        var command = new CancelSaleCommand(_faker.Random.Guid());

        var sale = GenerateRandomSale(command.Id);

        _saleRepository.GetByIdWithIncludeAsync(command.Id, Arg.Any<CancellationToken>(), Arg.Any<Expression<Func<Sale, object>>>())
            .Returns(Task.FromResult(sale));

        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(x => Task.FromResult(x.ArgAt<Sale>(0)));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("The sale was successfully cancelled.");

        _saleRepository.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
        _saleCancelledEventHandler.Received(1).Handle(Arg.Is<SaleCancelledEvent>(e => e.Sale.Id == command.Id), Arg.Any<CancellationToken>());
    }
}
