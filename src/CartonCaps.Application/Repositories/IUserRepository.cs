using CartonCaps.Domain.Entities;

namespace CartonCaps.Application.Repositories;
/// <summary>
/// Interface for user repository operations.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Gets a user by their ID.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>The user entity for the specified user ID.</returns>
    Task<User> GetUserByIdAsync(Guid userId);
}