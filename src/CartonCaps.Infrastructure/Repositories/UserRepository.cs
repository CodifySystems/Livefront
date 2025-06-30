using CartonCaps.Application.Repositories;
using CartonCaps.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CartonCaps.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly MockDbContext _context;

    // // Simulated in-memory data store
    // private readonly MockDbContext _context = new MockDbContext(
    //     new DbContextOptionsBuilder<MockDbContext>()
    //         .UseInMemoryDatabase("CartonCapsDatabase")
    //         .Options);

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