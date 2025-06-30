using CartonCaps.Domain.Entities;
using CartonCaps.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CartonCaps.Infrastructure;

public class MockDbContext : DbContext
{
    public MockDbContext(DbContextOptions<MockDbContext> options) : base(options)
    {
        // Ensure the database is created
        Database.EnsureCreated();
        // Seed initial data if necessary
        if (!Users.Any() && !Referrals.Any())
        {
            SeedData();
        }
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Referral> Referrals { get; set; } = null!;

    /// <summary>
    /// Configures the model for the mock database context with the User and Referral entities.
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().HasKey(u => u.UserId);
        modelBuilder.Entity<Referral>().HasKey(r => r.ReferralId);
    }

    public void SeedData()
    {
        // Seed users if they do not exist.
        if (!Users.Any())
        {
            Users.AddRange(new List<User>
            {
                new User { UserId = new Guid("29fd60d2-cf8b-4f93-ab8b-d9e5d768fc1a"), FirstName = "Alice", LastName = "Bag", Email = "alice.bag@annagram.io", ReferralCode = "AL1C3B" },
                new User { UserId = new Guid("b1c2d3e4-f5a6-7b8c-9d0e-f1a2b3c4d5e6"), FirstName = "Kathleen", LastName = "Hannah", Email = "kathleen.hannah@annagram.io", ReferralCode = "KT5Y8B" },
                new User { UserId = new Guid("c1d2e3f4-a5b6-7c8d-9e0f-1a2b3c4d5e6f"), FirstName = "Debbie", LastName = "Harry", Email = "debbie.harry@annagram.io", ReferralCode = "DE3H4R" },
                new User { UserId = new Guid("a80afede-b590-4c5a-a449-10d6c65d091c"), FirstName = "Joan", LastName = "Jett", Email = "joan.jett@annagram.io", ReferralCode = "JO4J3T" },
                new User { UserId = new Guid("fd9e60df-7b1c-41fa-8e96-ee561a7ee870"), FirstName = "Poly", LastName = "Styrene", Email = "poly.styrene@annagram.io", ReferralCode= "PO9S7R" }, 
            });
        }

        // Seed referrals if they do not exist.
        if (!Referrals.Any())
        {
            Referrals.AddRange(new List<Referral>
            {
                new Referral { ReferralId = new Guid("714c572a-4ff7-4801-8684-2672ade84c1b"), UserId = new Guid("29fd60d2-cf8b-4f93-ab8b-d9e5d768fc1a"), ReferredDeepLink = "https://cartoncaps.link/lo32ing90ab?referralCode=AL1C3B", Status = ReferralStatus.InProgress },
                new Referral { ReferralId = new Guid("13d2920e-f5c4-4ee6-a97a-1ff50d55eda8"), UserId = new Guid("29fd60d2-cf8b-4f93-ab8b-d9e5d768fc1a"), ReferredDeepLink = "https://cartoncaps.link/pqj82jabb9q?referralCode=AL1C3B", Status = ReferralStatus.Completed, ClaimedByUserId = new Guid("b1c2d3e4-f5a6-7b8c-9d0e-f1a2b3c4d5e6") },
                new Referral { ReferralId = new Guid("5c3f2acc-cabb-422d-89fa-d7d616ed382c"), UserId = new Guid("29fd60d2-cf8b-4f93-ab8b-d9e5d768fc1a"), ReferredDeepLink = "https://cartoncaps.link/lo32dq4abbe?referralCode=AL1C3B", Status = ReferralStatus.Abandoned },
                new Referral { ReferralId = new Guid("32e765a3-7cb7-4872-a1fe-1cebbe313300"), UserId = new Guid("b1c2d3e4-f5a6-7b8c-9d0e-f1a2b3c4d5e6"), ReferredDeepLink = "https://cartoncaps.link/ddb4int09u9?referralCode=KT5Y8B", Status = ReferralStatus.Completed, ClaimedByUserId = new Guid("c1d2e3f4-a5b6-7c8d-9e0f-1a2b3c4d5e6f") },
                new Referral { ReferralId = new Guid("37889b4f-6b59-46ef-87ac-4b5b9862414d"), UserId = new Guid("b1c2d3e4-f5a6-7b8c-9d0e-f1a2b3c4d5e6"), ReferredDeepLink = "https://cartoncaps.link/jqbdbnt76i9?referralCode=KT5Y8B", Status = ReferralStatus.InProgress },
            });
        }

        // Call SaveChanges to persist the seeded data to the database.
        SaveChanges();
    }
}