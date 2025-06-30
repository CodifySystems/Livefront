using CartonCaps.Application.Repositories;
using CartonCaps.Application.Services;
using CartonCaps.Application.Common.Exceptions;
using CartonCaps.Domain.Entities;
using CartonCaps.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CartonCaps.Infrastructure.Repositories;

/// <summary>
/// Repository for managing referrals in the system.
/// </summary>
public class ReferralRepository : IReferralRepository
{
    private readonly MockDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferralRepository"/> class.
    /// </summary>
    /// <param name="context">Database context to use for this object</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <remarks>Uses MockDbContext for testing purposes.</remarks>
    public ReferralRepository(MockDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Adds a new referral for a user by their UserId.
    /// </summary>
    /// <param name="userId">User ID if user adding referral</param>
    /// <returns>Referral entity of newly added referral</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NotFoundException"></exception>
    public async Task<Referral> AddReferralForUserAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentNullException("User Id cannot be empty.", nameof(userId));
        }

        // Check if user exists
        UserRepository userRepository = new UserRepository(_context);
        var query = userRepository.GetUserByIdAsync(userId);
        if (query == null || query.Result == null)
        {
            throw new NotFoundException("Specified user not found.");
        }

        // User record exists, proceed to create referral
        var user = query.Result;
        var referral = new Referral
        {
            UserId = user.UserId,
            ReferredDeepLink = DeepLinkService.GetDeepLink(user.UserId, user.ReferralCode),
            Status = ReferralStatus.InProgress // Default status
        };

        // Save referral to the database
        await _context.Referrals.AddAsync(referral);
        await _context.SaveChangesAsync();
        return referral;
    }

    /// <summary>
    /// Retrieves all referrals for a specific user by their UserId.
    /// </summary>
    /// <param name="userId">User ID to retrieve referrals for</param>
    /// <returns>List of Referral entities</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public Task<List<Referral>> GetReferralsByUserIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentNullException("User Id cannot be empty.", nameof(userId));
        }

        // Fetch referrals for the specified user
        return _context.Referrals
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }

    /// <summary>
    /// Updates the status of a referral.
    /// </summary>
    /// <param name="referralId">Idenfier for the referral</param>
    /// <param name="status">Referral status value</param>
    /// <returns>Claimed referral entity.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="BadRequestException"></exception>
    public Task<Referral> UpdateReferralStatusAsync(Guid referralId, ReferralStatus status)
    {
        if (referralId == Guid.Empty)
        {
            throw new ArgumentNullException("Referral Id cannot be empty.", nameof(referralId));
        }

        // Find the referral by Id
        var referral = _context.Referrals.FirstAsync(r => r.ReferralId == referralId);
        if (referral == null)
        {
            throw new ArgumentException("Referral not found.");
        }

        // Validate the status change
        if (status == ReferralStatus.Completed)
        {
            if (referral.Result.ClaimedByUserId == null || referral.Result.ClaimedByUserId == Guid.Empty)
            {
                throw new BadRequestException("Referral must be claimed before it can be marked as completed.");
            }
        }

        if (referral.Result.Status == ReferralStatus.Completed || referral.Result.Status == ReferralStatus.Abandoned)
        {
            throw new BadRequestException("Cannot update referral status of Completed or Abandoned referrals.");
        }
        else
        {
            // Update the status and timestamp
            referral.Result.Status = status;
            referral.Result.UpdateTimestamp();

            // Save changes to the database
            _context.Referrals.Update(referral.Result);
            _context.SaveChangesAsync();
        }

        return referral;
    }

    /// <summary>
    /// Claims a referral for a user.
    /// </summary>
    /// <param name="referralId">Idenfier for the referral</param>
    /// <param name="claimedByUserId">User ID for the user claiming this referral</param>
    /// <returns>Claimed referral entity.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="BadRequestException"></exception>
    public async Task<Referral> ClaimReferralAsync(Guid referralId, Guid claimedByUserId)
    {
        if (referralId == Guid.Empty)
        {
            throw new ArgumentNullException("Referral Id cannot be empty.", nameof(referralId));
        }

        if (claimedByUserId == Guid.Empty)
        {
            throw new ArgumentNullException("Claimant User Id cannot be empty.", nameof(claimedByUserId));
        }

        // Find the referral by Id
        var referral = await _context.Referrals.FirstOrDefaultAsync(r => r.ReferralId == referralId)
            ?? throw new BadRequestException("Referral not found.");

        // Check if the referral has already been claimed by another user
        if (referral.ClaimedByUserId != null && referral.ClaimedByUserId != Guid.Empty)
        {
            throw new BadRequestException("Referral has already been claimed.");
        }

        // Check if the referral is already claimed or abandoned
        if (referral.Status != ReferralStatus.InProgress)
        {
            throw new BadRequestException("Referral is not in a claimable state.");
        }

        // Check if the claimant user exists
        UserRepository userRepository = new UserRepository(_context);
        var claimantUser = await userRepository.GetUserByIdAsync(claimedByUserId);
        if (claimantUser == null)
        {
            throw new BadRequestException("Claimant user not found.");
        }
        else
        {
            // User record exists, proceed to claim referral
            var claimant = claimantUser;

            // Verify the claimant is not the same as the user who created the referral
            if (claimant.UserId == referral.UserId)
            {
                throw new BadRequestException("User cannot claim their own referral.");
            }
            // Verify the claimant is not already the claimed user
            if (referral.ClaimedByUserId == claimant.UserId)
            {
                throw new BadRequestException("Referral has already been claimed by this user.");
            }

            // Verify the claimant is not already associated with another active or completed referral
            if (_context.Referrals.Any(r => r.ClaimedByUserId == claimant.UserId && r.Status != ReferralStatus.Abandoned))
            {
                throw new BadRequestException("Claimant has already claimed a referral.");
            }

            // Update the status to Completed and set the user who claimed it
            referral.Status = ReferralStatus.Completed;
            referral.ClaimedByUserId = claimant.UserId;
            referral.ClaimedByUserName = claimant.ShortDisplayName; // Set the display name of the user who claimed the referral
            referral.UpdateTimestamp();

            // Save changes to the database
            _context.Referrals.Update(referral);
            await _context.SaveChangesAsync();

            return referral;
        }
    }
}