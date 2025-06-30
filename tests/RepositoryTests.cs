using CartonCaps.Application.Repositories;
using CartonCaps.Application.Common.Exceptions;
using CartonCaps.Domain.Entities;
using CartonCaps.Domain.Enums;
using Moq;

namespace CartonCaps.Tests;

public class RepositoryTests
{
    private Mock<IReferralRepository> _mockReferralRepository;

    [SetUp]
    public void Setup()
    {
        _mockReferralRepository = new Mock<IReferralRepository>();
    }

    [Test]
    public async Task GetReferralsByUserIdAsync_ReturnsReferrals_WhenUserIdIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var referrals = new List<Referral>
        {
            new Referral { UserId = userId, Status = ReferralStatus.InProgress, ReferredDeepLink = "https://example.com/referral" }
        };

        _mockReferralRepository.Setup(repo => repo.GetReferralsByUserIdAsync(userId))
            .ReturnsAsync(referrals);

        // Act
        var result = await _mockReferralRepository.Object.GetReferralsByUserIdAsync(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(userId, Is.EqualTo(result[0].UserId));
    }

    [Test]
    public async Task GetReferralsByUserIdAsync_ReturnsEmptyList_WhenUserIdHasNoReferrals()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mockReferralRepository.Setup(repo => repo.GetReferralsByUserIdAsync(userId))
            .ReturnsAsync(new List<Referral>());

        // Act
        var result = await _mockReferralRepository.Object.GetReferralsByUserIdAsync(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public void GetReferralsByUserIdAsync_ThrowsException_WhenUserIdIsInvalid()
    {
        // Arrange
        var userId = Guid.Empty;

        // Act & Assert
        _mockReferralRepository
            .Setup(repo => repo.GetReferralsByUserIdAsync(userId))
            .ThrowsAsync(new ArgumentNullException("User Id cannot be empty.", nameof(userId)));
    }
}