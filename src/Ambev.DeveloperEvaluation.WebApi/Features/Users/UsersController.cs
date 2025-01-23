using Ambev.DeveloperEvaluation.Application.Users.Commands.CreateUser;
using Ambev.DeveloperEvaluation.Application.Users.Commands.DeleteUser;
using Ambev.DeveloperEvaluation.Application.Users.Commands.UpdateUser;
using Ambev.DeveloperEvaluation.Application.Users.Queries.GetListUser;
using Ambev.DeveloperEvaluation.Application.Users.Queries.GetUser;
using Ambev.DeveloperEvaluation.Common.Utils;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.DeleteUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.GetListUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.UpdateUser;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users;

/// <summary>
/// Controller for managing user operations
/// </summary>
[ApiController]
[Route("api/users")]
public class UsersController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of UsersController
    /// </summary>
    /// <param name="mediator">The mediator instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public UsersController(
        IMediator mediator,
        IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves a user by their ID
    /// </summary>
    /// <param name="id">The unique identifier of the user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user details if found</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUser(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var request = new GetUserRequest { Id = id };

        var validator = new GetUserRequestValidator();

        var validationResult =
            await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var query = _mapper.Map<GetUserQuery>(request.Id);

        var response =
            await _mediator.Send(query, cancellationToken);

        return Ok(new ApiResponseWithData<GetUserResponse>
        {
            Success = true,
            Message = "User retrieved successfully",
            Data = _mapper.Map<GetUserResponse>(response)
        });
    }

    /// <summary>
    /// Retrieves all Users by pagination.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A response with the list of all Users.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<PaginatedResponse<GetListUserResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetListUser(
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetListUserQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var response =
            await _mediator.Send(query, cancellationToken);

        var paginatedList =
            new PaginatedList<GetListUserResponse>(_mapper.Map<List<GetListUserResponse>>(response),
                                                   response.TotalCount,
                                                   pageNumber,
                                                   pageSize);

        return OkPaginated(paginatedList);
    }

    /// <summary>
    /// Creates a new user
    /// </summary>
    /// <param name="request">The user creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created user details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateUserResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var validator = new CreateUserRequestValidator();

        var validationResult =
            await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<CreateUserCommand>(request);

        var response =
            await _mediator.Send(command, cancellationToken);

        return Created(string.Empty, new ApiResponseWithData<CreateUserResponse>
        {
            Success = true,
            Message = "User created successfully",
            Data = _mapper.Map<CreateUserResponse>(response)
        });
    }

    /// <summary> 
    /// Updates an existing user 
    /// </summary> 
    /// <param name="id">The unique identifier of the user to update</param> 
    /// <param name="request">The user update request</param> 
    /// <param name="cancellationToken">Cancellation token</param> 
    /// <returns>The updated user details</returns> 
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(
        [FromRoute] Guid id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var validator = new UpdateUserRequestValidator();

        var validationResult =
            await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<UpdateUserCommand>(request);

        command.Id = id;

        var response =
            await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<UpdateUserResponse>
        {
            Success = true,
            Message = "User updated successfully",
            Data = _mapper.Map<UpdateUserResponse>(response)
        });
    }

    /// <summary>
    /// Deletes a user by their ID
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response if the user was deleted</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var request = new DeleteUserRequest { Id = id };

        var validator = new DeleteUserRequestValidator();

        var validationResult =
            await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<DeleteUserCommand>(request.Id);

        await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "User deleted successfully"
        });
    }
}