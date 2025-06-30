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

        // _context.Database.EnsureCreated();

        // // Seed initial data if necessary
        // if (!_context.Users.Any())
        // {
        //     _context.SeedData();
        // }
    }

    /// <summary>
    /// Retrieves a user by their unique identifier (UserId).
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public Task<User> GetUserByIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentNullException("User Id cannot be empty.", nameof(userId));
        }

        // Simulate fetching a user from a database or service
        return _context.Users.FirstAsync(u => u.UserId == userId);
    }
}