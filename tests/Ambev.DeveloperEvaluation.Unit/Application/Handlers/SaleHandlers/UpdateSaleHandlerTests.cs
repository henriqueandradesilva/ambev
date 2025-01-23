using Ambev.DeveloperEvaluation.Application.Sales.Commands.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.Events.Handlers;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.MessageBroker.Interfaces;
using Ambev.DeveloperEvaluation.Unit.Application.Handlers.SaleHandlers.TestData;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using System.Linq.Expressions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Handlers.SaleHandlers;

public class UpdateSaleHandlerTests : SaleHandlerTest
{
    private readonly ISaleRepository _saleRepository;
    private readonly SaleModifiedEventHandler _saleModifiedEventHandler;
    private readonly UpdateSaleHandler _handler;
    private readonly IEventPublisher _eventPublisher;

    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _eventPublisher = Substitute.For<IEventPublisher>();
        _saleModifiedEventHandler = new SaleModifiedEventHandler(_eventPublisher);
        _handler = new UpdateSaleHandler(_saleRepository, _saleModifiedEventHandler);
    }

    [Fact(DisplayName = "Should throw ValidationException when the command is invalid")]
    public async Task Handle_ShouldThrowValidationException_WhenCommandIsInvalid()
    {
        // Arrange
        var command = new UpdateSaleCommand();

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact(DisplayName = "Should throw KeyNotFoundException when sale does not exist")]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenSaleDoesNotExist()
    {
        // Arrange
        var command = GenerateValidUpdateCommand();

        _saleRepository.GetByIdWithIncludeAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>(), Arg.Any<Expression<Func<Sale, object>>>())
            .Returns(Task.FromResult<Sale>(null));

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"Sale with ID {command.Id} not found");
    }

    [Fact(DisplayName = "Should throw InvalidOperationException when sale is already cancelled")]
    public async Task Handle_ShouldThrowInvalidOperationException_WhenSaleIsCancelled()
    {
        // Arrange
        var command = GenerateValidUpdateCommand();
        var existingSale = GenerateFakeSale(command);
        existingSale.CancelSale(true);

        _saleRepository.GetByIdWithIncludeAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>(), Arg.Any<Expression<Func<Sale, object>>>())
            .Returns(Task.FromResult<Sale>(existingSale));

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Sale is already cancelled.");
    }

    [Fact(DisplayName = "Should update sale properties correctly")]
    public async Task Handle_ShouldUpdateSalePropertiesCorrectly()
    {
        // Arrange
        var command = GenerateValidUpdateCommand();
        var existingSale = GenerateFakeSale(command);

        _saleRepository.GetByIdWithIncludeAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>(), Arg.Any<Expression<Func<Sale, object>>>())
            .Returns(Task.FromResult<Sale>(existingSale));

        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(x => Task.FromResult(x.ArgAt<Sale>(0)));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Id.Should().Be(command.Id);
        existingSale.Customer.Should().Be(command.Customer);
        existingSale.Branch.Should().Be(command.Branch);
        existingSale.Date.Should().Be(command.Date);
    }

    [Fact(DisplayName = "Should publish SaleModifiedEvent when sale is updated")]
    public async Task Handle_ShouldPublishSaleModifiedEvent_WhenSaleIsUpdated()
    {
        // Arrange
        var command = GenerateValidUpdateCommand();
        var existingSale = GenerateFakeSale(command);

        _saleRepository.GetByIdWithIncludeAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>(), Arg.Any<Expression<Func<Sale, object>>>())
            .Returns(Task.FromResult<Sale>(existingSale));

        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(x => Task.FromResult(x.ArgAt<Sale>(0)));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _eventPublisher.Received(1).PublishToQueueAsync(Arg.Any<SaleModifiedEvent>(), "SaleModifiedEvent");
    }
}