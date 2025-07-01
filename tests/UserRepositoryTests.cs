using CartonCaps.Application.Common.Exceptions;
using CartonCaps.Application.Repositories;
using CartonCaps.Application.Services;
using CartonCaps.Domain.Entities;
using CartonCaps.Domain.Enums;
using Moq;

namespace CartonCaps.Tests;

public class UserRepositoryTests
{
    private Mock<IUserRepository> _mockUserRepository;

    [SetUp]
    public void Setup()
    {
        _mockUserRepository = new Mock<IUserRepository>();
    }

    [Test]
    [Description("Get user by ID")]
    public async Task GetUserByIdAsync_ReturnsUser_WhenUserIdIsValid()
    {
        // Arrange
        var userId = new Guid("29fd60d2-cf8b-4f93-ab8b-d9e5d768fc1a");
        var expectedUser = new User
        {
            UserId = userId,
            FirstName = "Alice",
            LastName = "Bag",
            Email = "alice.bag@annagram.io",
            ReferralCode = "AL1C3B"
        };

        _mockUserRepository
            .Setup(repo => repo.GetUserByIdAsync(userId))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _mockUserRepository.Object.GetUserByIdAsync(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.UserId, Is.EqualTo(userId));
        Assert.That(result.ShortDisplayName, Is.EqualTo("Alice B."));
        Assert.That(result.ReferralCode, Is.EqualTo("AL1C3B"));
    }

    [Test]
    [Description("Get user by ID throws NotFoundException when user does not exist")]
    public void GetUserByIdAsync_ThrowsNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mockUserRepository
            .Setup(repo => repo.GetUserByIdAsync(userId))
            .ThrowsAsync(new NotFoundException("User not found"));

        // Act & Assert
        var exception = Assert.ThrowsAsync<NotFoundException>(async () =>
            await _mockUserRepository.Object.GetUserByIdAsync(userId));

        Assert.That(exception.Message, Is.EqualTo("User not found"));
    }

    [Test]
    [Description("Get user by ID throws ArgumentNullException when userId is empty guid")]
    public void GetUserByIdAsync_ReturnsNull_WhenUserIdIsEmpty()
    {
        // Arrange
        var userId = Guid.Empty;

        // Setup the mock to throw an ArgumentNullException
        _mockUserRepository
            .Setup(repo => repo.GetUserByIdAsync(userId))
            .ThrowsAsync(new ArgumentNullException("User Id cannot be empty.", nameof(userId)));

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _mockUserRepository.Object.GetUserByIdAsync(userId));
    }

    [TearDown]
    public void TearDown()
    {
        _mockUserRepository = null!;
    }
}