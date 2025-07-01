using CartonCaps.Application.Common.Exceptions;
using CartonCaps.Application.Repositories;
using CartonCaps.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CartonCaps.Infrastructure.Repositories;

/// <summary>
/// Repository for managing user data in the system.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly MockDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepository"/> class.
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public UserRepository(MockDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Retrieves a user by their unique identifier (UserId).
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<User> GetUserByIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentNullException("User Id cannot be empty.", nameof(userId));
        }

        // Retrieve the user by UserId
        var result = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        if (result == null || result.UserId == Guid.Empty)
        {
            throw new NotFoundException("User not found.");
        }
        else
        {
            return result;
        }
    }
}