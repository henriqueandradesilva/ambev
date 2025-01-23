using Ambev.DeveloperEvaluation.Application.Sales.Queries.GetListSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetListSale;

/// <summary>
/// Profile for AutoMapper configuration for sales related mappings.
/// </summary>
public class GetListSaleProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetListSaleProfile"/> class.
    /// </summary>
    public GetListSaleProfile()
    {
        CreateMap<Sale, GetListSaleResult>();
        CreateMap<SaleItem, GetListSaleItemResult>();
        CreateMap<GetListSaleResult, GetListSaleResponse>();
        CreateMap<GetListSaleItemResult, GetListSaleItemResponse>();
    }
}