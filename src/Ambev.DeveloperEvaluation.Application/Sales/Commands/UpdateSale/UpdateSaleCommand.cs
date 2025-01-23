using Ambev.DeveloperEvaluation.Common.Validation;
using MediatR;

/// <summary>
/// Represents a command to update an existing sale, including its properties and validation logic.
/// </summary>
namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.UpdateSale;

public class UpdateSaleCommand : IRequest<UpdateSaleResult>
{
    public UpdateSaleCommand()
    {

    }

    public UpdateSaleCommand(
        Guid id,
        string customer,
        string branch,
        string number,
        DateTime? date,
        bool isCancelled)
    {
        Id = id;
        Customer = customer;
        Branch = branch;
        Number = number;

        if (date != null)
            Date = date.Value;

        IsCancelled = isCancelled;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the sale to be updated.
    /// </summary>
    public Guid Id { get; set; }

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
    /// Indicates whether the sale is cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Validates the command using the <see cref="UpdateSaleCommandValidator"/> and returns the validation results.
    /// </summary>
    /// <returns>A <see cref="ValidationResultDetail"/> containing the validation status and any errors.</returns>
    public ValidationResultDetail Validate()
    {
        var validator = new UpdateSaleCommandValidator();

        var result = validator.Validate(this);

        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}