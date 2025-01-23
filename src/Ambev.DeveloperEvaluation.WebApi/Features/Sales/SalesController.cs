using Ambev.DeveloperEvaluation.Application.Sales.Commands.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.Commands.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.Queries.GetListSale;
using Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSale;
using Ambev.DeveloperEvaluation.Common.Utils;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetListSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

/// <summary>
/// Controller for managing sale operations
/// </summary>
[ApiController]
[Route("api/sales")]
public class SalesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public SalesController(
        IMediator mediator,
        IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves a specific sale by ID.
    /// </summary>
    /// <param name="id">The unique identifier of the sale.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The requested sale details.</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSale(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var request = new GetSaleRequest { Id = id };

        var validator = new GetSaleRequestValidator();

        var validationResult =
            await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var query = _mapper.Map<GetSaleQuery>(request);

        var response =
            await _mediator.Send(query, cancellationToken);

        return Ok(new ApiResponseWithData<GetSaleResponse>
        {
            Success = true,
            Message = "The sale was retrieved successfully.",
            Data = _mapper.Map<GetSaleResponse>(response)
        });
    }

    /// <summary>
    /// Retrieves a paginated list of sales.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve (default is 1).</param>
    /// <param name="pageSize">The number of items per page (default is 10).</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A paginated list of sales.</returns>
    [HttpGet]
    public async Task<IActionResult> GetListSale(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetListSaleQuery(pageNumber, pageSize);

        var response =
            await _mediator.Send(query, cancellationToken);

        var paginatedList =
            new PaginatedList<GetListSaleResponse>(_mapper.Map<List<GetListSaleResponse>>(response),
                                                   response.TotalCount,
                                                   pageNumber,
                                                   pageSize);

        return OkPaginated(paginatedList);
    }

    /// <summary>
    /// Creates a new sale.
    /// </summary>
    /// <param name="request">The sale creation request.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The created sale.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateSale(
        [FromBody] CreateSaleRequest request,
        CancellationToken cancellationToken)
    {
        var validator = new CreateSaleRequestValidator();

        var validationResult =
            await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<CreateSaleCommand>(request);

        var response =
            await _mediator.Send(command, cancellationToken);

        return Created(string.Empty, new ApiResponseWithData<CreateSaleResponse>
        {
            Success = true,
            Message = "The sale was created successfully.",
            Data = _mapper.Map<CreateSaleResponse>(response)
        });
    }

    /// <summary>
    /// Updates an existing sale.
    /// </summary>
    /// <param name="id">The unique identifier of the sale to update.</param>
    /// <param name="request">The sale update request.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>The updated sale.</returns>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateSale(
        [FromRoute] Guid id,
        [FromBody] UpdateSaleRequest request,
        CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleRequestValidator();

        var validationResult =
            await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<UpdateSaleCommand>(request);

        command.Id = id;

        var response =
            await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<UpdateSaleResponse>
        {
            Success = true,
            Message = "The sale was updated successfully.",
            Data = _mapper.Map<UpdateSaleResponse>(response)
        });
    }

    /// <summary>
    /// Deletes an existing sale.
    /// </summary>
    /// <param name="id">The unique identifier of the sale to delete.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A confirmation message.</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteSale(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var request = new DeleteSaleRequest { Id = id };

        var validator = new DeleteSaleRequestValidator();

        var validationResult =
            await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<DeleteSaleCommand>(request.Id);

        await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "The sale was deleted successfully."
        });
    }

    /// <summary>
    /// Cancels an entire sale.
    /// </summary>
    /// <param name="id">The unique identifier of the sale to cancel.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A confirmation message.</returns>
    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> CancelSale(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = _mapper.Map<CancelSaleCommand>(id);

        await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "The sale was cancelled successfully."
        });
    }

    /// <summary>
    /// Cancels a specific item in a sale.
    /// </summary>
    /// <param name="saleId">The unique identifier of the sale.</param>
    /// <param name="saleItemId">The unique identifier of the item to cancel.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A confirmation message.</returns>
    [HttpPatch("{id:guid}/items/{saleItemId:guid}/cancel")]
    public async Task<IActionResult> CancelItem(
        [FromRoute] Guid id,
        [FromRoute] Guid saleItemId,
        CancellationToken cancellationToken)
    {
        var command = new CancelSaleItemCommand(id, saleItemId);

        await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "The item was cancelled successfully."
        });
    }
}