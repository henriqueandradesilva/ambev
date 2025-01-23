using Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Handlers.SaleHandlers.TestData;
using AutoMapper;
using Bogus;
using FluentAssertions;
using NSubstitute;
using System.Linq.Expressions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Handlers.SaleHandlers;

public class GetSaleHandlerTests : SaleHandlerTest
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSaleHandler _handler;
    private readonly Faker _faker;

    public GetSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSaleHandler(_saleRepository, _mapper);
        _faker = new Faker();
    }

    [Fact(DisplayName = "Should return validation error when ID is empty")]
    public async Task Validate_EmptyId_ReturnsErrorMessage()
    {
        // Arrange
        var query = new GetSaleQuery(Guid.Empty);

        var validator = new GetSaleQueryValidator();
        var validationResult = await validator.ValidateAsync(query, CancellationToken.None);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.PropertyName == "Id" && e.ErrorMessage == "Sale ID is required");
    }

    [Fact(DisplayName = "Should throw KeyNotFoundException when sale is not found")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var query = new GetSaleQuery(_faker.Random.Guid());

        _saleRepository.GetByIdWithIncludeAsync(query.Id, Arg.Any<CancellationToken>(), Arg.Any<Expression<Func<Sale, object>>>())
            .Returns(Task.FromResult<Sale>(null));

        // Act
        var act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {query.Id} not found");
    }

    [Fact(DisplayName = "Should return sale when request is valid")]
    public async Task Handle_ValidRequest_ReturnsSale()
    {
        // Arrange
        var query = new GetSaleQuery(_faker.Random.Guid());
        var sale = GenerateRandomSale(query.Id);
        var saleResult = GenerateRandomGetSaleResult(query.Id);

        _saleRepository.GetByIdWithIncludeAsync(query.Id, Arg.Any<CancellationToken>(), Arg.Any<Expression<Func<Sale, object>>>())
            .Returns(Task.FromResult(sale));

        _mapper.Map<GetSaleResult>(Arg.Any<Sale>())
            .Returns(saleResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(query.Id);
        result.TotalAmount.Should().Be(saleResult.TotalAmount);

        _saleRepository.Received(1).GetByIdWithIncludeAsync(query.Id, Arg.Any<CancellationToken>(), Arg.Any<Expression<Func<Sale, object>>>());
    }
}
