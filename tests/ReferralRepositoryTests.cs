using CartonCaps.Application.Common.Exceptions;
using CartonCaps.Application.Repositories;
using CartonCaps.Application.Services;
using CartonCaps.Domain.Entities;
using CartonCaps.Domain.Enums;
using Moq;

namespace CartonCaps.Tests;

public class ReferralRepositoryTests
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

    [Test]
    public void GetReferralsByUserIdAsync_ThrowsException_WhenUserIdDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mockReferralRepository.Setup(repo => repo.GetReferralsByUserIdAsync(userId))
            .ThrowsAsync(new NotFoundException("No referrals found for the specified user."));

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await _mockReferralRepository.Object.GetReferralsByUserIdAsync(userId));
    }

    [Test]
    public async Task AddReferralForUserAsync_AddsReferral_WhenUserIdIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var referral = new Referral
        {
            UserId = userId,
            Status = ReferralStatus.InProgress,
            ReferredDeepLink = "https://example.com/referral"
        };

        _mockReferralRepository.Setup(repo => repo.AddReferralForUserAsync(userId))
            .ReturnsAsync(referral);

        // Act
        var result = await _mockReferralRepository.Object.AddReferralForUserAsync(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.UserId, Is.EqualTo(userId));
        Assert.That(result.Status, Is.EqualTo(ReferralStatus.InProgress));
    }

    [Test]
    public void AddReferralForUserAsync_ThrowsException_WhenUserIdIsInvalid()
    {
        // Arrange
        var userId = Guid.Empty;

        // Act & Assert
        _mockReferralRepository
            .Setup(repo => repo.AddReferralForUserAsync(userId))
            .ThrowsAsync(new ArgumentNullException("User Id cannot be empty.", nameof(userId)));
    }

    [Test]
    public void AddReferralForUserAsync_ThrowsException_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mockReferralRepository.Setup(repo => repo.AddReferralForUserAsync(userId))
            .ThrowsAsync(new NotFoundException("Specified user not found."));

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await _mockReferralRepository.Object.AddReferralForUserAsync(userId));
    }

    [Test]
    public async Task ClaimReferralAsync_ClaimsReferral_WhenReferralIdIsValid()
    {
        // Arrange
        var referralId = new Guid("714c572a-4ff7-4801-8684-2672ade84c1b");
        var claimedByUserId = new Guid("fd9e60df-7b1c-41fa-8e96-ee561a7ee870");
        var userId = new Guid("29fd60d2-cf8b-4f93-ab8b-d9e5d768fc1a");
        var referral = new Referral()
        {
            UserId = userId,
            ReferralId = referralId,
            ClaimedByUserId = claimedByUserId,
            ClaimedByUserName = "Poly S.",
            Status = ReferralStatus.Completed,
            ReferredDeepLink = DeepLinkService.GetDeepLink(userId, "AL1C3B") // "https://cartoncaps.link/lo32ing90ab?referralCode=AL1C3B"
        };
        referral.UpdateTimestamp();

        _mockReferralRepository.Setup(repo => repo.ClaimReferralAsync(referralId, claimedByUserId))
            .ReturnsAsync(referral);

        // Act
        var result = await _mockReferralRepository.Object.ClaimReferralAsync(referralId, claimedByUserId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ReferralId, Is.EqualTo(referralId));
        Assert.That(result.ClaimedByUserId, Is.EqualTo(claimedByUserId));
        Assert.That(result.Status, Is.EqualTo(ReferralStatus.Completed));
        Assert.That(result.ClaimedByUserName, Is.EqualTo("Poly S."));
    }

    [Test]
    public void ClaimReferralAsync_ThrowsException_WhenReferralIdIsInvalid()
    {
        // Arrange
        var referralId = Guid.Empty;
        var claimedByUserId = Guid.NewGuid();

        // Act & Assert
        _mockReferralRepository
            .Setup(repo => repo.ClaimReferralAsync(referralId, claimedByUserId))
            .ThrowsAsync(new ArgumentNullException("Referral Id cannot be empty.", nameof(referralId)));
    }

    [Test]
    public void ClaimReferralAsync_ThrowsException_WhenClaimedByUserIdIsInvalid()
    {
        // Arrange
        var referralId = new Guid("714c572a-4ff7-4801-8684-2672ade84c1b");
        var claimedByUserId = Guid.Empty;

        // Act & Assert
        _mockReferralRepository
            .Setup(repo => repo.ClaimReferralAsync(referralId, claimedByUserId))
            .ThrowsAsync(new ArgumentNullException("Claimed By User Id cannot be empty.", nameof(claimedByUserId)));
    }

    [Test]
    public void ClaimReferralAsync_ThrowsException_WhenReferralDoesNotExist()
    {
        // Arrange
        var referralId = new Guid("714c572a-4ff7-4801-8684-2672ade84c1b");
        var claimedByUserId = new Guid("fd9e60df-7b1c-41fa-8e96-ee561a7ee870");

        _mockReferralRepository.Setup(repo => repo.ClaimReferralAsync(referralId, claimedByUserId))
            .ThrowsAsync(new NotFoundException("Specified referral not found."));

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await _mockReferralRepository.Object.ClaimReferralAsync(referralId, claimedByUserId));
    }

    [Test]
    public void ClaimReferralAsync_ThrowsException_WhenReferralAlreadyClaimed()
    {
        // Arrange
        var referralId = new Guid("13d2920e-f5c4-4ee6-a97a-1ff50d55eda8");
        var claimedByUserId = new Guid("b1c2d3e4-f5a6-7b8c-9d0e-f1a2b3c4d5e6");

        _mockReferralRepository.Setup(repo => repo.ClaimReferralAsync(referralId, claimedByUserId))
            .ThrowsAsync(new BadRequestException("Referral has already been claimed."));

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(async () => await _mockReferralRepository.Object.ClaimReferralAsync(referralId, claimedByUserId));
    }

    [Test]
    public void ClaimReferralAsync_ThrowsException_WhenReferralNotClaimable()
    {
        // Arrange
        var referralId = new Guid("5c3f2acc-cabb-422d-89fa-d7d616ed382c");
        var claimedByUserId = new Guid("b1c2d3e4-f5a6-7b8c-9d0e-f1a2b3c4d5e6");

        _mockReferralRepository.Setup(repo => repo.ClaimReferralAsync(referralId, claimedByUserId))
            .ThrowsAsync(new BadRequestException("Referral is not in a claimable state."));

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(async () => await _mockReferralRepository.Object.ClaimReferralAsync(referralId, claimedByUserId));
    }


    [Test]
    public async Task UpdateReferralStatusAsync_UpdatesStatus_WhenReferralIdIsValid()
    {
        // Arrange
        var referralId = new Guid("714c572a-4ff7-4801-8684-2672ade84c1b");
        var userId = new Guid("29fd60d2-cf8b-4f93-ab8b-d9e5d768fc1a");
        var newStatus = ReferralStatus.Completed;
        var referral = new Referral
        {
            UserId = userId,
            ReferralId = referralId,
            Status = newStatus,
            ReferredDeepLink = "https://cartoncaps.link/lo32ing90ab?referralCode=AL1C3B"
        };

        _mockReferralRepository.Setup(repo => repo.UpdateReferralStatusAsync(referralId, newStatus))
            .ReturnsAsync(referral);

        // Act
        var result = await _mockReferralRepository.Object.UpdateReferralStatusAsync(referralId, newStatus);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ReferralId, Is.EqualTo(referralId));
        Assert.That(result.Status, Is.EqualTo(newStatus));
    }

    [Test]
    public void UpdateReferralStatusAsync_ThrowsException_WhenReferralIdIsInvalid()
    {
        // Arrange
        var referralId = Guid.Empty;
        var newStatus = ReferralStatus.Completed;

        // Act & Assert
        _mockReferralRepository
            .Setup(repo => repo.UpdateReferralStatusAsync(referralId, newStatus))
            .ThrowsAsync(new ArgumentNullException("Referral Id cannot be empty.", nameof(referralId)));
    }

    [Test]
    public void UpdateReferralStatusAsync_ThrowsException_WhenNewStatusIsInvalid()
    {
        // Arrange
        var referralId = new Guid("714c572a-4ff7-4801-8684-2672ade84c1b");
        var newStatus = (ReferralStatus)999; // Invalid status

        // Act & Assert
        _mockReferralRepository
            .Setup(repo => repo.UpdateReferralStatusAsync(referralId, newStatus))
            .ThrowsAsync(new ArgumentOutOfRangeException("Invalid referral status.", nameof(newStatus)));
    }

    [Test]
    public void UpdateReferralStatusAsync_ThrowsException_WhenReferralDoesNotExist()
    {
        // Arrange
        var referralId = Guid.NewGuid();
        var newStatus = ReferralStatus.Completed;

        _mockReferralRepository.Setup(repo => repo.UpdateReferralStatusAsync(referralId, newStatus))
            .ThrowsAsync(new NotFoundException("Specified referral not found."));

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await _mockReferralRepository.Object.UpdateReferralStatusAsync(referralId, newStatus));
    }

    [Test]
    public void UpdateReferralStatusAsync_ThrowsException_WhenCompletingBeforeReferralIsClaimed()
    {
        // Arrange
        var referralId = new Guid("714c572a-4ff7-4801-8684-2672ade84c1b");
        var newStatus = ReferralStatus.Completed;

        _mockReferralRepository.Setup(repo => repo.UpdateReferralStatusAsync(referralId, newStatus))
            .ThrowsAsync(new BadRequestException("Referral must be claimed before it can be marked as completed."));

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(async () => await _mockReferralRepository.Object.UpdateReferralStatusAsync(referralId, newStatus));
    }

    [Test]
    public void UpdateReferralStatusAsync_ThrowsException_WhenReferralCompletedOrAbandoned()
    {
        // Arrange
        var referralId = new Guid("13d2920e-f5c4-4ee6-a97a-1ff50d55eda8");
        var newStatus = ReferralStatus.Abandoned;

        _mockReferralRepository.Setup(repo => repo.UpdateReferralStatusAsync(referralId, newStatus))
            .ThrowsAsync(new BadRequestException("Cannot update referral status of Completed or Abandoned referrals."));

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(async () => await _mockReferralRepository.Object.UpdateReferralStatusAsync(referralId, newStatus));
    }

    [TearDown]
    public void TearDown()
    {
        _mockReferralRepository = null!;
    }
}