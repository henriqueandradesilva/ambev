using Ambev.DeveloperEvaluation.Common.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Common;

public class BaseEntity : IComparable<BaseEntity>
{
    public Guid Id { get; set; }

    /// <summary>
    /// Gets the date and time when the sale was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the date and time when the sale was last updated, if applicable.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; } = true;

    #region Extensions

    public void MarkAsUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsIsActive(
        bool active)
    {
        IsActive = active;
        UpdatedAt = DateTime.UtcNow;
    }

    #endregion

    public Task<IEnumerable<ValidationErrorDetail>> ValidateAsync()
    {
        return Validator.ValidateAsync(this);
    }

    public int CompareTo(
        BaseEntity? other)
    {
        if (other == null)
            return 1;

        return other!.Id.CompareTo(Id);
    }
}