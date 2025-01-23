using Ambev.DeveloperEvaluation.Application.Sales.Commands.DeleteSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;

/// <summary>
/// AutoMapper profile for mapping between a Guid and the DeleteSaleCommand object.
/// </summary>
public class DeleteSaleProfile : Profile
{
    /// <summary>
    /// Configures the mapping for the DeleteSale feature.
    /// </summary>
    public DeleteSaleProfile()
    {
        // Maps a Guid to DeleteSaleCommand by constructing an instance using the Guid as a parameter.
        CreateMap<Guid, DeleteSaleCommand>()
            .ConstructUsing(id => new DeleteSaleCommand(id));
    }
}