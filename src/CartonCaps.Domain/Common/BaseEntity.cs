namespace CartonCaps.Domain.Common;

/// <summary>
/// Base class for all entities in the domain.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Created at timestamp.
    /// </summary>
    /// <returns>Creation date and time.</returns>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Updated at timestamp.
    /// </summary>
    /// <returns>Last update date and time.</returns>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Updates the UpdatedAt timestamp to the current UTC time.
    /// </summary>
    public void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}