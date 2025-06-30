using CartonCaps.Domain.Entities;
using CartonCaps.Domain.Enums;

namespace CartonCaps.Application.Repositories;

/// <summary>
/// Interface for managing referrals in the system.
/// </summary>
public interface IReferralRepository
{
    /// <summary>
    /// Retrieves a list of referrals for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A list of referrals associated with the specified user.</returns>
    Task<List<Referral>> GetReferralsByUserIdAsync(Guid userId);

    /// <summary>
    /// Adds a new referral for a user.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>The newly created referral entity.</returns>
    Task<Referral> AddReferralForUserAsync(Guid userId);

    /// <summary>
    /// Updates the status of a referral.
    /// </summary>
    /// <param name="referralId">The unique identifier of the referral.</param>
    /// <param name="status">The new status to set for the referral.</param>
    /// <returns>The updated referral entity.</returns>
    Task<Referral> UpdateReferralStatusAsync(Guid referralId, ReferralStatus status);

    /// <summary>
    /// This method is used when a user claims a referral, marking it as claimed.
    /// </summary>
    /// <param name="referralId">The unique identifier of the referral to be claimed.</param>
    /// <param name="claimedByUserId">The unique identifier of the user claiming the referral.</param>
    /// <returns>The claimed referral entity.</returns>
    Task<Referral> ClaimReferralAsync(Guid referralId, Guid claimedByUserId);
}