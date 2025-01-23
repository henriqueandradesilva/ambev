using Ambev.DeveloperEvaluation.Application.Sales.Commands.UpdateSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// AutoMapper profile for mapping between the sale update request, command, and response.
/// </summary>
public class UpdateSaleProfile : Profile
{
    /// <summary>
    /// Initializes the mapping configurations for sale update.
    /// </summary>
    public UpdateSaleProfile()
    {
        CreateMap<UpdateSaleRequest, UpdateSaleCommand>();
        CreateMap<UpdateSaleResult, UpdateSaleResponse>();
    }
}