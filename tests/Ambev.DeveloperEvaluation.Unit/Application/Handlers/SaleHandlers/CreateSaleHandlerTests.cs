using Ambev.DeveloperEvaluation.Application.Sales.Commands.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.Events.Handlers;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.MessageBroker.Interfaces;
using Ambev.DeveloperEvaluation.Unit.Application.Handlers.SaleHandlers.TestData;
using FluentAssertions;
using Moq;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Handlers.SaleHandlers;

public class CreateSaleHandlerTests : SaleHandlerTest
{
    private readonly ISaleRepository _saleRepository;
    private readonly SaleCreatedEventHandler _saleCreatedEventHandler;
    private readonly CreateSaleHandler _handler;
    private readonly IEventPublisher _eventPublisher;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _eventPublisher = Substitute.For<IEventPublisher>();
        _saleCreatedEventHandler = new SaleCreatedEventHandler(_eventPublisher);
        _handler = new CreateSaleHandler(_saleRepository, _saleCreatedEventHandler);
    }

    [Fact(DisplayName = "Should throw ValidationException when command is invalid")]
    public async Task Handle_ShouldThrowValidationException_WhenCommandIsInvalid()
    {
        // Arrange
        var command = new CreateSaleCommand();

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "Should publish SaleCreatedEvent when sale is created")]
    public async Task Handle_ShouldPublishEvent_WhenSaleIsCreated()
    {
        // Arrange
        var command = GenerateValidCommand();
        var sale = GenerateFakeSale(command);

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(sale));

        var mockPublisher = new Mock<IEventPublisher>();
        var eventHandler = new SaleCreatedEventHandler(mockPublisher.Object);
        var handler = new CreateSaleHandler(_saleRepository, eventHandler);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        mockPublisher.Verify(ep =>
            ep.PublishToQueueAsync(It.IsAny<SaleCreatedEvent>(), "SaleCreatedEvent"), Times.Once);
    }

    [Fact(DisplayName = "Should calculate total amount for valid sale items with discounts")]
    public async Task Handle_ShouldCalculateTotalAmount_WhenValidSaleItemsProvided()
    {
        // Arrange
        var command = GenerateValidCommand();
        var sale = GenerateFakeSale(command);

        sale.CalculateTotalAmount();

        var expectedTotal = sale.ListSaleItems
            .Sum(item =>
            {
                var discountRate = item.Quantity switch
                {
                    >= 10 => 0.8m, // 20% de desconto
                    >= 4 => 0.9m,  // 10% de desconto
                    _ => 1.0m      // Sem desconto
                };
                return item.Quantity * item.UnitPrice * discountRate;
            });

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(sale));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        sale.TotalAmount.Should().Be(expectedTotal);
    }

    [Theory(DisplayName = "Should apply correct discount based on item quantity")]
    [InlineData(4, 10)]
    [InlineData(10, 20)]
    public async Task Handle_ShouldApplyCorrectDiscountBasedOnQuantity(int quantity, decimal discountPercentage)
    {
        // Arrange
        var command = GenerateValidCommand();
        command.ListSaleItems = new List<CreateSaleItemCommand>
        {
            new CreateSaleItemCommand { Product = "Product A", Quantity = quantity, UnitPrice = 100 }
        };

        var sale = GenerateFakeSale(command);
        sale.CalculateTotalAmount();

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(sale));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var saleItem = sale.ListSaleItems.First();
        var expectedDiscountedPrice = saleItem.Quantity * saleItem.UnitPrice * ((100 - discountPercentage) / 100);
        saleItem.TotalAmount.Should().Be(expectedDiscountedPrice);
    }

    [Fact(DisplayName = "Should throw ValidationException when quantity exceeds limit")]
    public async Task Handle_ShouldThrowValidationException_WhenQuantityExceedsLimit()
    {
        // Arrange
        var command = GenerateValidCommand();
        command.ListSaleItems = new List<CreateSaleItemCommand>
        {
            new CreateSaleItemCommand { Product = "Product A", Quantity = 21, UnitPrice = 100 }
        };

        // Act
        var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(
            () => _handler.Handle(command, CancellationToken.None));

        // Assert
        exception.Errors.Should().Contain(e => e.ErrorMessage.Contains("You can only purchase up to 20 units of the same product."));
    }

    [Fact(DisplayName = "Should not apply discount when quantity is below threshold")]
    public async Task Handle_ShouldNotApplyDiscount_WhenQuantityIsBelowThreshold()
    {
        // Arrange
        var command = GenerateValidCommand();
        command.ListSaleItems = new List<CreateSaleItemCommand>
        {
            new CreateSaleItemCommand { Product = "Product A", Quantity = 3, UnitPrice = 100 }
        };

        var sale = GenerateFakeSale(command);
        sale.CalculateTotalAmount();

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(sale));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var saleItem = sale.ListSaleItems.First();
        var expectedTotal = saleItem.Quantity * saleItem.UnitPrice;
        saleItem.TotalAmount.Should().Be(expectedTotal);
    }
}
