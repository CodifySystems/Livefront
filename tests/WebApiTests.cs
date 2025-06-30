using CartonCaps.Application.Common.Exceptions;
using CartonCaps.Application.Repositories;
using CartonCaps.Domain.Entities;
using CartonCaps.WebAPI.Controllers;
using CartonCaps.WebAPI.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CartonCaps.Tests;

public class WebApiTests
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
                    Status = Domain.Enums.ReferralStatus.InProgress,
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
        Assert.That(response.Referrals[0].Status, Is.EqualTo(Domain.Enums.ReferralStatus.InProgress));
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
}
