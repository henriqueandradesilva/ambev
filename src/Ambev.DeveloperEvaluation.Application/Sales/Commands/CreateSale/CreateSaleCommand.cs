using Ambev.DeveloperEvaluation.Common.Validation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.CreateSale;

/// <summary>
/// Command to create a new sale.
/// </summary>
/// <remarks>
/// This command is part of the CQRS pattern and encapsulates the data required 
/// to create a new sale, including customer details, branch, sale number, date, 
/// and the list of sale items.
/// </remarks>
public class CreateSaleCommand : IRequest<CreateSaleResult>
{
    public CreateSaleCommand()
    {

    }

    public CreateSaleCommand(
        string customer,
        string branch,
        string number,
        DateTime? date)
    {
        Customer = customer;
        Branch = branch;
        Number = number;

        if (date != null)
            Date = date.Value;
    }

    /// <summary>
    /// Gets or sets the customer associated with the sale.
    /// </summary>
    public string Customer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch where the sale occurred.
    /// </summary>
    public string Branch { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier for the sale.
    /// </summary>
    public string Number { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date of the sale.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the list of items included in the sale.
    /// </summary>
    public List<CreateSaleItemCommand> ListSaleItems { get; set; } = new();

    /// <summary>
    /// Validates the current command using the <see cref="CreateSaleCommandValidator"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="ValidationResultDetail"/> indicating whether the command is valid 
    /// and containing any validation errors.
    /// </returns>
    public ValidationResultDetail Validate()
    {
        var validator = new CreateSaleCommandValidator();

        var result = validator.Validate(this);

        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}