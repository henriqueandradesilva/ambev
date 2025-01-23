namespace Ambev.DeveloperEvaluation.Application.Common;

/// <summary>
/// Represents a base class for result objects, encapsulating the success status and an optional message.
/// </summary>
public class BaseResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets an optional message providing additional information about the result.
    /// </summary>
    public string? Message { get; set; }
}
