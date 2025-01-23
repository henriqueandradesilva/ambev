using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSale;

/// <summary>
/// Query for retrieving a Sale by their ID.
/// </summary>
public class GetSaleQuery : IRequest<GetSaleResult>
{
    /// <summary>
    /// The unique identifier of the Sale to retrieve
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Initializes a new instance of the GetSaleQuery.
    /// </summary>
    /// <param name="id">The ID of the Sale to retrieve</param>
    public GetSaleQuery(
        Guid id)
    {
        Id = id;
    }
}