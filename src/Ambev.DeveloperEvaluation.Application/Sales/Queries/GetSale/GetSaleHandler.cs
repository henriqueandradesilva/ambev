using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using Serilog;

namespace Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSale;

/// <summary>
/// Handler for processing GetSaleQuery requests
/// </summary>
public class GetSaleHandler : IRequestHandler<GetSaleQuery, GetSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of GetSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public GetSaleHandler(
        ISaleRepository saleRepository,
        IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the GetSaleQuery request
    /// </summary>
    /// <param name="request">The GetSale query</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The Sale details if found</returns>
    public async Task<GetSaleResult> Handle(
        GetSaleQuery request,
        CancellationToken cancellationToken)
    {
        var validator = new GetSaleQueryValidator();

        var validationResult =
            await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            var message = string.Join(Environment.NewLine, validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));

            Log.Error("Validation failed: {ValidationErrors}", message);

            throw new ValidationException(validationResult.Errors);
        }

        var sale =
            await _saleRepository.GetByIdWithIncludeAsync(request.Id, cancellationToken, x => x.ListSaleItems);

        if (sale == null)
        {
            var message = $"Sale with ID {request.Id} not found";

            Log.Error(message);

            throw new KeyNotFoundException(message);
        }

        return _mapper.Map<GetSaleResult>(sale);
    }
}