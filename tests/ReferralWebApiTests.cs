using CartonCaps.Application.Common.Exceptions;
using CartonCaps.Application.Repositories;
using CartonCaps.Application.Services;
using CartonCaps.Domain.Entities;
using CartonCaps.Domain.Enums;
using CartonCaps.WebAPI.Controllers;
using CartonCaps.WebAPI.ResponseModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CartonCaps.Tests;

public class ReferralWebApiTests
{
    private Mock<IReferralRepository> _mockReferralRepository;

    [SetUp]
    public void Setup()
    {
        _mockReferralRepository = new Mock<IReferralRepository>();
    }

    [Test]
    [Description("Get referrals for a user")]
    public async Task GetReferralsTestAsync_ReturnsReferrals_WhenUserIdIsValid()
    {
        // Arrange
        var userId = new Guid("29fd60d2-cf8b-4f93-ab8b-d9e5d768fc1a");

        _mockReferralRepository
            .Setup(repo => repo.GetReferralsByUserIdAsync(userId))
            .ReturnsAsync(new List<Referral>
            {
                new Referral
                {
                    ReferralId = Guid.NewGuid(),
                    UserId = userId,
                    Status = ReferralStatus.InProgress,
                    CreatedAt = DateTime.UtcNow,
                    ReferredDeepLink = "https://example.com/referral"
                }
            });

        var controller = new ReferralController(_mockReferralRepository.Object);

        // Act
        var result = await controller.GetReferrals(userId);

        // Assert
        Assert.That(result, Is.InstanceOf<IActionResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value, Is.InstanceOf<ReferralListResponse>());
        var response = okResult.Value as ReferralListResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Status, Is.EqualTo("Success"));
        Assert.That(response.Referrals, Is.Not.Null);
        Assert.That(response.Referrals.Count, Is.GreaterThan(0));
        Assert.That(response.Referrals[0].UserId, Is.EqualTo(userId));
        Assert.That(response.Referrals[0].Status, Is.EqualTo(ReferralStatus.InProgress));
    }

    [Test]
    [Description("Get referrals for a user with no referrals")]
    public async Task GetReferralsTestAsync_ReturnsNotFound_WhenNoReferralsExist()
    {
        // Arrange
        var userId = new Guid("29fd60d2-cf8b-4f93-ab8b-d9e5d768fc1a");

        _mockReferralRepository
            .Setup(repo => repo.GetReferralsByUserIdAsync(userId))
            .ReturnsAsync(new List<Referral>());

        var controller = new ReferralController(_mockReferralRepository.Object);

        // Act
        var result = await controller.GetReferrals(userId);

        // Assert
        Assert.That(result, Is.InstanceOf<IActionResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
        Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
        Assert.That(notFoundResult.Value, Is.InstanceOf<BadRequestResponse>());
        var response = notFoundResult.Value as BadRequestResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Message, Is.EqualTo("No referrals found for this user."));
    }

    [Test]
    [Description("Get referrals for a user with invalid UserId")]
    public async Task GetReferralsTestAsync_ReturnsNotFound_WhenUserIdIsInvalid()
    {
        // Arrange
        var userId = Guid.NewGuid(); // Non-existent user ID;

        _mockReferralRepository
            .Setup(repo => repo.GetReferralsByUserIdAsync(userId));

        var controller = new ReferralController(_mockReferralRepository.Object);

        // Act
        var result = await controller.GetReferrals(userId);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
        Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
        Assert.That(notFoundResult.Value, Is.InstanceOf<BadRequestResponse>());
        var response = notFoundResult.Value as BadRequestResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Message, Is.EqualTo("No referrals found for this user."));
    }

    [Test]
    [Description("Get referrals for a user with empty UserId")]
    public async Task GetReferralsTestAsync_ReturnsUserIdCannotBeEmpty_WhenUserIdIsEmpty()
    {
        // Arrange
        var userId = Guid.Empty;

        _mockReferralRepository
            .Setup(repo => repo.GetReferralsByUserIdAsync(userId));

        var controller = new ReferralController(_mockReferralRepository.Object);

        // Act
        var result = await controller.GetReferrals(userId);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        Assert.That(badRequestResult.Value, Is.InstanceOf<BadRequestResponse>());
        var response = badRequestResult.Value as BadRequestResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Message, Is.EqualTo("User ID cannot be empty."));
    }

    [Test]
    [Description("Add referral for a user")]
    public async Task AddReferralTestAsync_ReturnsReferral_WhenUserIdIsValid()
    {
        // Arrange
        var userId = new Guid("29fd60d2-cf8b-4f93-ab8b-d9e5d768fc1a");

        _mockReferralRepository
            .Setup(repo => repo.AddReferralForUserAsync(userId))
            .ReturnsAsync(new Referral
            {
                ReferralId = Guid.NewGuid(),
                UserId = userId,
                Status = ReferralStatus.InProgress,
                CreatedAt = DateTime.UtcNow,
                ReferredDeepLink = DeepLinkService.GetDeepLink(userId, "AL1C3B") // Example deep link
            });

        var controller = new ReferralController(_mockReferralRepository.Object);

        // Act
        var result = await controller.CreateReferral(userId);

        // Assert
        Assert.That(result, Is.InstanceOf<IActionResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value, Is.InstanceOf<NewReferralResponse>());
        var response = okResult.Value as NewReferralResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Status, Is.EqualTo("Success"));
        Assert.That(response.ReferralId, Is.Not.Empty);
    }

    [Test]
    [Description("Add referral for a user with invalid UserId")]
    public async Task AddReferralTestAsync_ReturnsNotFound_WhenUserIdIsInvalid()
    {
        // Arrange
        var userId = Guid.NewGuid(); // Non-existent user ID

        _mockReferralRepository
            .Setup(repo => repo.AddReferralForUserAsync(userId));

        var controller = new ReferralController(_mockReferralRepository.Object);

        // Act
        var result = await controller.CreateReferral(userId);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
        Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
        Assert.That(notFoundResult.Value, Is.InstanceOf<BadRequestResponse>());
        var response = notFoundResult.Value as BadRequestResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Message, Is.EqualTo("Referral could not be created."));
    }

    [Test]
    [Description("Add referral for a user with empty UserId")]
    public async Task AddReferralTestAsync_ReturnsBadRequest_WhenUserIdIsEmpty()
    {
        // Arrange
        var userId = Guid.Empty;

        _mockReferralRepository
            .Setup(repo => repo.AddReferralForUserAsync(userId));

        var controller = new ReferralController(_mockReferralRepository.Object);

        // Act
        var result = await controller.CreateReferral(userId);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        Assert.That(badRequestResult.Value, Is.InstanceOf<BadRequestResponse>());
        var response = badRequestResult.Value as BadRequestResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Message, Is.EqualTo("User ID cannot be empty."));
    }

    [Test]
    [Description("Claim referral for a user")]
    public async Task ClaimReferralTestAsync_ReturnsReferralUpdated_WhenReferralIdIsValid()
    {
        // Arrange
        var userId = new Guid("29fd60d2-cf8b-4f93-ab8b-d9e5d768fc1a");
        var referralId = new Guid("714c572a-4ff7-4801-8684-2672ade84c1b");
        var claimedByUserId = new Guid("fd9e60df-7b1c-41fa-8e96-ee561a7ee870");

        _mockReferralRepository
            .Setup(repo => repo.ClaimReferralAsync(referralId, claimedByUserId))
            .ReturnsAsync(new Referral
            {
                UserId = userId,
                ReferralId = referralId,
                ClaimedByUserId = claimedByUserId,
                Status = ReferralStatus.Completed,
                ClaimedByUserName = "Poly S.",
                ReferredDeepLink = DeepLinkService.GetDeepLink(claimedByUserId, "AL1C3B") // Example deep link
            });

        var controller = new ReferralController(_mockReferralRepository.Object);

        // Act
        var result = await controller.ClaimReferral(referralId, claimedByUserId);

        // Assert
        Assert.That(result, Is.InstanceOf<IActionResult>());
        var acceptedResult = result as AcceptedResult;
        Assert.That(acceptedResult, Is.Not.Null);
        Assert.That(acceptedResult.StatusCode, Is.EqualTo(202));
        Assert.That(acceptedResult.Value, Is.InstanceOf<ReferralUpdatedResponse>());
        var response = acceptedResult.Value as ReferralUpdatedResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Status, Is.EqualTo("Success"));
        Assert.That(response.ReferralId, Is.EqualTo(referralId));
        Assert.That(response.Message, Is.EqualTo("Referral claimed successfully."));
        Assert.That(response.NewStatus, Is.EqualTo(ReferralStatus.Completed));
    }

    [Test]
    [Description("Claim referral for a user with invalid ReferralId")]
    public async Task ClaimReferralTestAsync_ReturnsNotFound_WhenReferralIdIsInvalid()
    {
        // Arrange
        var referralId = Guid.NewGuid(); // Non-existent referral ID
        var claimedByUserId = new Guid("fd9e60df-7b1c-41fa-8e96-ee561a7ee870");

        var controller = new ReferralController(_mockReferralRepository.Object);

        // Act 
        var result = await controller.ClaimReferral(referralId, claimedByUserId);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
        Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
        Assert.That(notFoundResult.Value, Is.InstanceOf<BadRequestResponse>());
        var response = notFoundResult.Value as BadRequestResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Message, Is.EqualTo("Referral not found."));
    }

    [Test]
    [Description("Claim referral for a user with empty ReferralId")]
    public async Task ClaimReferralTestAsync_ReturnsBadRequest_WhenReferralIdIsEmpty()
    {
        // Arrange
        var referralId = Guid.Empty; // Non-existent referral ID
        var claimedByUserId = new Guid("fd9e60df-7b1c-41fa-8e96-ee561a7ee870");

        var controller = new ReferralController(_mockReferralRepository.Object);

        // Act
        var result = await controller.ClaimReferral(referralId, claimedByUserId);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        Assert.That(badRequestResult.Value, Is.InstanceOf<BadRequestResponse>());
        var response = badRequestResult.Value as BadRequestResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Message, Is.EqualTo("Referral ID and Claimed by User ID cannot be empty."));
    }

    [Test]
    [Description("Claim referral for a user with empty ClaimedByUserId")]
    public async Task ClaimReferralTestAsync_ReturnsBadRequest_WhenClaimedByUserIdIsEmpty()
    {
        // Arrange
        var referralId = new Guid("714c572a-4ff7-4801-8684-2672ade84c1b");
        var claimedByUserId = Guid.Empty;

        var controller = new ReferralController(_mockReferralRepository.Object);

        // Act
        var result = await controller.ClaimReferral(referralId, claimedByUserId);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        Assert.That(badRequestResult.Value, Is.InstanceOf<BadRequestResponse>());
        var response = badRequestResult.Value as BadRequestResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Message, Is.EqualTo("Referral ID and Claimed by User ID cannot be empty."));
    }

    [Test]
    [Description("Claim referral for a user that has already claimed a referral")]
    public async Task ClaimReferralTestAsync_ReturnsBadRequest_WhenUserHasAlreadyClaimedReferral()
    {
        // Arrange
        var referralId = new Guid("714c572a-4ff7-4801-8684-2672ade84c1b");
        var claimedByUserId = new Guid("fd9e60df-7b1c-41fa-8e96-ee561a7ee870");

        _mockReferralRepository
            .Setup(repo => repo.ClaimReferralAsync(referralId, claimedByUserId))
            .Throws(new BadRequestException("Claimant has already claimed a referral."));

        var controller = new ReferralController(_mockReferralRepository.Object);

      // Act
        var result = await controller.ClaimReferral(referralId, claimedByUserId);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        Assert.That(badRequestResult.Value, Is.InstanceOf<BadRequestResponse>());
        var response = badRequestResult.Value as BadRequestResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Message, Is.EqualTo("Claimant has already claimed a referral."));
    }

    [Test]
    [Description("Claim referral for a user that has already claimed this referral")]
    public async Task ClaimReferralTestAsync_ReturnsBadRequest_WhenUserHasAlreadyClaimedThisReferral()
    {
        // Arrange
        var referralId = new Guid("714c572a-4ff7-4801-8684-2672ade84c1b");
        var claimedByUserId = new Guid("fd9e60df-7b1c-41fa-8e96-ee561a7ee870");

        _mockReferralRepository
            .Setup(repo => repo.ClaimReferralAsync(referralId, claimedByUserId))
            .Throws(new BadRequestException("Referral has already been claimed by this user."));

        var controller = new ReferralController(_mockReferralRepository.Object);

        // Act
        var result = await controller.ClaimReferral(referralId, claimedByUserId);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        Assert.That(badRequestResult.Value, Is.InstanceOf<BadRequestResponse>());
        var response = badRequestResult.Value as BadRequestResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Message, Is.EqualTo("Referral has already been claimed by this user."));
    }

    [Test]
    [Description("Claim referral for a user that is the same as the user who created the referral")]
    public async Task ClaimReferralTestAsync_ReturnsBadRequest_WhenUserClaimsOwnReferral()
    {
        // Arrange
        var referralId = new Guid("714c572a-4ff7-4801-8684-2672ade84c1b");
        var claimedByUserId = new Guid("29fd60d2-cf8b-4f93-ab8b-d9e5d768fc1a"); // Same as user who created the referral

        _mockReferralRepository
            .Setup(repo => repo.ClaimReferralAsync(referralId, claimedByUserId))
            .Throws(new BadRequestException("User cannot claim their own referral."));

        var controller = new ReferralController(_mockReferralRepository.Object);

        // Act
        var result = await controller.ClaimReferral(referralId, claimedByUserId);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        Assert.That(badRequestResult.Value, Is.InstanceOf<BadRequestResponse>());
        var response = badRequestResult.Value as BadRequestResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Message, Is.EqualTo("User cannot claim their own referral."));
    }

    [Test]
    [Description("Claim referral for a user that does not exist")]
    public async Task ClaimReferralTestAsync_ReturnsNotFound_WhenClaimantUserDoesNotExist()
    {
        // Arrange
        var referralId = new Guid("714c572a-4ff7-4801-8684-2672ade84c1b");
        var claimedByUserId = Guid.NewGuid(); // Non-existent user

        _mockReferralRepository
            .Setup(repo => repo.ClaimReferralAsync(referralId, claimedByUserId))
            .Throws(new NotFoundException("Claimant user not found."));

        var controller = new ReferralController(_mockReferralRepository.Object);

        // Act
        var result = await controller.ClaimReferral(referralId, claimedByUserId);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
        Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
        Assert.That(notFoundResult.Value, Is.InstanceOf<BadRequestResponse>());
        var response = notFoundResult.Value as BadRequestResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Message, Is.EqualTo("Claimant user not found."));
    }

    [Test]
    [Description("Update referral status for a user")]
    public async Task UpdateReferralStatusTestAsync_ReturnsUpdatedReferral_WhenStatusIsValid()
    {
        // Arrange
        var userId = new Guid("29fd60d2-cf8b-4f93-ab8b-d9e5d768fc1a");
        var referralId = new Guid("714c572a-4ff7-4801-8684-2672ade84c1b");
        var newStatus = ReferralStatus.Completed;

        _mockReferralRepository
            .Setup(repo => repo.UpdateReferralStatusAsync(referralId, newStatus))
            .ReturnsAsync(new Referral
            {
                UserId = userId,
                ReferralId = referralId,
                Status = newStatus,
                ClaimedByUserName = "Poly S.",
                ReferredDeepLink = DeepLinkService.GetDeepLink(userId, "AL1C3B")
            });

        var controller = new ReferralController(_mockReferralRepository.Object);

        // Act
        var result = await controller.UpdateReferralStatus(referralId, newStatus);

        // Assert
        Assert.That(result, Is.InstanceOf<IActionResult>());
        var acceptedResult = result as AcceptedResult;
        Assert.That(acceptedResult, Is.Not.Null);
        Assert.That(acceptedResult.StatusCode, Is.EqualTo(202));
        Assert.That(acceptedResult.Value, Is.InstanceOf<ReferralUpdatedResponse>());
        var response = acceptedResult.Value as ReferralUpdatedResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Status, Is.EqualTo("Success"));
        Assert.That(response.ReferralId, Is.EqualTo(referralId));
        Assert.That(response.Message, Is.EqualTo("Referral status updated successfully."));
        Assert.That(response.NewStatus, Is.EqualTo(newStatus));
    }

    [Test]
    [Description("Update referral status for a user with ReferralId not in database")]
    public async Task UpdateReferralStatusTestAsync_ReturnsNotFound_WhenReferralIdDoesNotExist()
    {
        // Arrange
        var referralId = Guid.NewGuid();
        var newStatus = ReferralStatus.Completed;

        var controller = new ReferralController(_mockReferralRepository.Object);

        // Act
        var result = await controller.UpdateReferralStatus(referralId, newStatus);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That(notFoundResult, Is.Not.Null);
        Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
        Assert.That(notFoundResult.Value, Is.InstanceOf<BadRequestResponse>());
        var response = notFoundResult.Value as BadRequestResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Message, Is.EqualTo("Referral not found."));
    }

    [Test]
    [Description("Update referral status for a user with empty ReferralId")]
    public async Task UpdateReferralStatusTestAsync_ReturnsBadRequest_WhenReferralIdIsEmpty()
    {
        // Arrange
        var referralId = Guid.Empty;
        var newStatus = ReferralStatus.Completed;

        var controller = new ReferralController(_mockReferralRepository.Object);

        // Act
        var result = await controller.UpdateReferralStatus(referralId, newStatus);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        Assert.That(badRequestResult.Value, Is.InstanceOf<BadRequestResponse>());
        var response = badRequestResult.Value as BadRequestResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Message, Is.EqualTo("Referral ID cannot be empty."));
    }

    [Test]
    [Description("Update referral status for a completed or abanded referral")]
    public async Task UpdateReferralStatusTestAsync_ReturnsBadRequest_WhenStatusIsInvalid()
    {
        // Arrange
        var referralId = new Guid("5c3f2acc-cabb-422d-89fa-d7d616ed382c");
        var newStatus = ReferralStatus.InProgress;

        _mockReferralRepository
            .Setup(repo => repo.UpdateReferralStatusAsync(referralId, newStatus))
            .Throws(new BadRequestException("Cannot update referral status of Completed or Abandoned referrals."));

        var controller = new ReferralController(_mockReferralRepository.Object);

        // Act
        var result = await controller.UpdateReferralStatus(referralId, newStatus);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        Assert.That(badRequestResult.Value, Is.InstanceOf<BadRequestResponse>());
        var response = badRequestResult.Value as BadRequestResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Message, Is.EqualTo("Cannot update referral status of Completed or Abandoned referrals."));
    }

    [Test]
    [Description("Update referral status for a referral that has not been claimed")]
    public async Task UpdateReferralStatusTestAsync_ReturnsBadRequest_WhenReferralNotClaimed()
    {
        // Arrange
        var referralId = new Guid("5c3f2acc-cabb-422d-89fa-d7d616ed382c");
        var newStatus = ReferralStatus.Completed;

        _mockReferralRepository
            .Setup(repo => repo.UpdateReferralStatusAsync(referralId, newStatus))
            .Throws(new BadRequestException("Referral must be claimed before it can be marked as completed."));

        var controller = new ReferralController(_mockReferralRepository.Object);

        // Act
        var result = await controller.UpdateReferralStatus(referralId, newStatus);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult, Is.Not.Null);
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));
        Assert.That(badRequestResult.Value, Is.InstanceOf<BadRequestResponse>());
        var response = badRequestResult.Value as BadRequestResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Message, Is.EqualTo("Referral must be claimed before it can be marked as completed."));
    }

    [Test]
    [Description("Trigger an exception in the referral repository")]
    public async Task TriggerExceptionInReferralRepository_ReturnsInternalServerError()
    {
        // Arrange
        var userId = new Guid("29fd60d2-cf8b-4f93-ab8b-d9e5d768fc1a");

        _mockReferralRepository
            .Setup(repo => repo.GetReferralsByUserIdAsync(userId))
            .Throws(new Exception("Database connection error"));

        var controller = new ReferralController(_mockReferralRepository.Object);

        // Act
        var result = await controller.GetReferrals(userId);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        Assert.That(objectResult.Value, Is.InstanceOf<ErrorResponse>());
        var response = objectResult.Value as ErrorResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Message, Is.EqualTo("An unexpected error occurred."));
    }

    [Test]
    [Description("Trigger an exception in the referral repository while claiming a referral")]
    public async Task TriggerExceptionInReferralRepositoryWhileClaimingReferral_ReturnsInternalServerError()
    {
        // Arrange
        var referralId = new Guid("714c572a-4ff7-4801-8684-2672ade84c1b");
        var claimedByUserId = new Guid("fd9e60df-7b1c-41fa-8e96-ee561a7ee870");

        _mockReferralRepository
            .Setup(repo => repo.ClaimReferralAsync(referralId, claimedByUserId))
            .Throws(new Exception("Database connection error"));

        var controller = new ReferralController(_mockReferralRepository.Object);

        // Act
        var result = await controller.ClaimReferral(referralId, claimedByUserId);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        Assert.That(objectResult.Value, Is.InstanceOf<ErrorResponse>());
        var response = objectResult.Value as ErrorResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Message, Is.EqualTo("An unexpected error occurred."));
    }

    [Test]
    [Description("Trigger an exception in the referral repository while updating a referral")]
    public async Task TriggerExceptionInReferralRepositoryWhileUpdatingReferral_ReturnsInternalServerError()
    {
        // Arrange
        var referralId = new Guid("714c572a-4ff7-4801-8684-2672ade84c1b");
        var claimedByUserId = new Guid("fd9e60df-7b1c-41fa-8e96-ee561a7ee870");

        _mockReferralRepository
            .Setup(repo => repo.UpdateReferralStatusAsync(referralId, ReferralStatus.Completed))
            .Throws(new Exception("Database connection error"));

        var controller = new ReferralController(_mockReferralRepository.Object);

        // Act
        var result = await controller.UpdateReferralStatus(referralId, ReferralStatus.Completed);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        Assert.That(objectResult.Value, Is.InstanceOf<ErrorResponse>());
        var response = objectResult.Value as ErrorResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Message, Is.EqualTo("An unexpected error occurred."));
    }

    [Test]
    [Description("Trigger an exception in the referral repository while creating a referral")]
    public async Task TriggerExceptionInReferralRepositoryWhileCreatingReferral_ReturnsInternalServerError()
    {
        // Arrange
        var userId = new Guid("fd9e60df-7b1c-41fa-8e96-ee561a7ee870");

        _mockReferralRepository
            .Setup(repo => repo.AddReferralForUserAsync(userId))
            .Throws(new Exception("Database connection error"));

        var controller = new ReferralController(_mockReferralRepository.Object);

        // Act
        var result = await controller.CreateReferral(userId);

        // Assert
        Assert.That(result, Is.InstanceOf<ObjectResult>());
        var objectResult = result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        Assert.That(objectResult.Value, Is.InstanceOf<ErrorResponse>());
        var response = objectResult.Value as ErrorResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Message, Is.EqualTo("An unexpected error occurred."));
    }

    [Test]
    [Description("Get list of referral statuses")]
    public void GetReferralStatuses_ReturnsListOfStatuses()
    {
        // Arrange
        
        var statusList = Enum.GetValues(typeof(ReferralStatus))
            .Cast<ReferralStatus>()
            .Select(status => new { Name = status.ToString(), Value = (int)status })
            .ToList();
        var controller = new ReferralController(_mockReferralRepository.Object);

        // Act
        var result = controller.GetReferralStatuses() as OkObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.StatusCode, Is.EqualTo(200));
#pragma warning disable CS8604 // Possible null reference argument.
        var resultObject = result.Value.AsDynamicObject();
#pragma warning restore CS8604 // Possible null reference argument.
        Assert.That(resultObject, Is.Not.Null);
        Assert.That(resultObject.Count, Is.EqualTo(statusList.Count));
        for (int i = 0; i < statusList.Count; i++)
        {
            Assert.That(resultObject[i].Name, Is.EqualTo(statusList[i].Name));
            Assert.That(resultObject[i].Value, Is.EqualTo(statusList[i].Value));
        }
    }

    [Test]
    [Description("Call API Health Check")]
    public void HealthCheck_ReturnsOk()
    {
        // Arrange
        var controller = new StatusController();

        // Act
        var result = controller.GetStatus();

        // Assert
        Assert.That(result, Is.InstanceOf<IActionResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value, Is.InstanceOf<SuccessResponse>());
        var response = okResult.Value as SuccessResponse;
        Assert.That(response, Is.Not.Null);
        Assert.That(response.Status, Is.EqualTo("Success"));
        Assert.That(response.Message, Is.EqualTo("CartonCaps API is running smoothly!"));
    }


    [TearDown]
    public void TearDown()
    {
        _mockReferralRepository = null!;
    }
}
