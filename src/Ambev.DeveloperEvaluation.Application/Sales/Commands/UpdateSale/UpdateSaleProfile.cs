using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.UpdateSale;

/// <summary>
/// Defines the AutoMapper profile for mapping between sale-related entities and DTOs.
/// </summary>
public class UpdateSaleProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSaleProfile"/> class.
    /// Configures mappings between <see cref="Sale"/>, <see cref="UpdateSaleResult"/>, and <see cref="UpdateSaleCommand"/>.
    /// </summary>
    public UpdateSaleProfile()
    {
        CreateMap<Sale, UpdateSaleResult>();
        CreateMap<UpdateSaleCommand, Sale>();
    }
}