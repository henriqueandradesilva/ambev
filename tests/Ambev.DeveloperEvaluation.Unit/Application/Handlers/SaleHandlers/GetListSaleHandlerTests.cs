using Ambev.DeveloperEvaluation.Application.Sales.Queries.GetListSale;
using Ambev.DeveloperEvaluation.Common.Utils;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Handlers.SaleHandlers.TestData;
using AutoMapper;
using Bogus;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Handlers.SaleHandlers;

public class GetListSaleHandlerTests : SaleHandlerTest
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetListSaleHandler _handler;
    private readonly Faker _faker;

    public GetListSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetListSaleHandler(_saleRepository, _mapper);
        _faker = new Faker();
    }

    [Fact(DisplayName = "Handle_WhenNoSalesExist_ReturnsEmptyPaginatedList")]
    public async Task Handle_WhenNoSalesExist_ReturnsEmptyPaginatedList()
    {
        var query = new GetListSaleQuery(pageNumber: 1, pageSize: 10);

        var sales = new List<Sale>();

        var paginatedSales = new PaginatedList<Sale>(sales ?? new List<Sale>(), 0, query.PageNumber, query.PageSize);

        _saleRepository.GetListSaleWithPaginationAsync(query.PageNumber, query.PageSize, Arg.Any<Func<IQueryable<Sale>, IQueryable<Sale>>>(), Arg.Any<CancellationToken>())
                       .Returns(Task.FromResult(paginatedSales));

        var mappedSales = new List<GetListSaleResult>();
        _mapper.Map<List<GetListSaleResult>>(Arg.Is<List<Sale>>(s => s.Count == 0)).Returns(mappedSales);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeOfType<PaginatedList<GetListSaleResult>>();
        result.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.PageSize.Should().Be(query.PageSize);
        result.CurrentPage.Should().Be(query.PageNumber);
        result.TotalPages.Should().Be(0);

        _saleRepository.Received(1).GetListSaleWithPaginationAsync(query.PageNumber, query.PageSize, Arg.Any<Func<IQueryable<Sale>, IQueryable<Sale>>>(), Arg.Any<CancellationToken>());
        _mapper.Received(1).Map<List<GetListSaleResult>>(Arg.Is<List<Sale>>(s => s.Count == 0));
    }

    [Fact(DisplayName = "Handle_WhenSalesExist_ReturnsPaginatedListWithSales")]
    public async Task Handle_WhenSalesExist_ReturnsPaginatedListWithSales()
    {
        var query = new GetListSaleQuery(pageNumber: 1, pageSize: 2);

        var sales = new List<Sale>
        {
            GenerateRandomSale(),
            GenerateRandomSale()
        };

        var paginatedSales = new PaginatedList<Sale>(sales, sales.Count, query.PageNumber, query.PageSize);

        _saleRepository.GetListSaleWithPaginationAsync(
            query.PageNumber,
            query.PageSize,
            Arg.Any<Func<IQueryable<Sale>, IQueryable<Sale>>>(),
            Arg.Any<CancellationToken>()
        ).Returns(Task.FromResult(paginatedSales));

        var mappedSales = sales.Select(s => new GetListSaleResult
        {
            Id = s.Id,
            TotalAmount = s.TotalAmount
        }).ToList();

        _mapper.Map<List<GetListSaleResult>>(Arg.Is<List<Sale>>(s => s.Count == 2)).Returns(mappedSales);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeOfType<PaginatedList<GetListSaleResult>>();
        result.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.PageSize.Should().Be(query.PageSize);
        result.CurrentPage.Should().Be(query.PageNumber);
        result.TotalPages.Should().Be(1);

        _saleRepository.Received(1).GetListSaleWithPaginationAsync(
            query.PageNumber,
            query.PageSize,
            Arg.Any<Func<IQueryable<Sale>, IQueryable<Sale>>>(),
            Arg.Any<CancellationToken>()
        );

        _mapper.Received(1).Map<List<GetListSaleResult>>(Arg.Is<List<Sale>>(s => s.Count == 2));
    }

    [Fact(DisplayName = "Handle_WhenPaginationApplied_ReturnsCorrectPageOfSales")]
    public async Task Handle_WhenPaginationApplied_ReturnsCorrectPageOfSales()
    {
        var query = new GetListSaleQuery(pageNumber: 2, pageSize: 2);

        var sales = new List<Sale>
        {
            GenerateRandomSale(),
            GenerateRandomSale(),
            GenerateRandomSale(),
            GenerateRandomSale(),
            GenerateRandomSale()
        };

        var paginatedSales = new PaginatedList<Sale>(sales.Skip(2).Take(2).ToList(), 5, query.PageNumber, query.PageSize);

        _saleRepository.GetListSaleWithPaginationAsync(query.PageNumber, query.PageSize, Arg.Any<Func<IQueryable<Sale>, IQueryable<Sale>>>(), Arg.Any<CancellationToken>())
                       .Returns(Task.FromResult(paginatedSales));

        var mappedSales = sales.Skip(2).Take(2).Select(s => new GetListSaleResult
        {
            Id = s.Id,
            TotalAmount = s.TotalAmount
        }).ToList();

        _mapper.Map<List<GetListSaleResult>>(Arg.Is<List<Sale>>(s => s.Count == 2)).Returns(mappedSales);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeOfType<PaginatedList<GetListSaleResult>>();
        result.Should().HaveCount(2);
        result.TotalCount.Should().Be(5);
        result.PageSize.Should().Be(query.PageSize);
        result.CurrentPage.Should().Be(query.PageNumber);
        result.TotalPages.Should().Be(3);

        _saleRepository.Received(1).GetListSaleWithPaginationAsync(query.PageNumber, query.PageSize, Arg.Any<Func<IQueryable<Sale>, IQueryable<Sale>>>(), Arg.Any<CancellationToken>());
        _mapper.Received(1).Map<List<GetListSaleResult>>(Arg.Is<List<Sale>>(s => s.Count == 2));
    }
}
