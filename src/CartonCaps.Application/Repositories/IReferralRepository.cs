using CartonCaps.Domain.Entities;
using CartonCaps.Domain.Enums;

namespace CartonCaps.Application.Repositories;

public interface IReferralRepository : IBaseRepository<Referral>
{

    /// <summary>
    /// Retrieves a list of referrals for a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A list of referrals associated with the specified user.</returns>
    Task<List<Referral>> GetReferralsByUserIdAsync(Guid userId);

    Task<Referral> AddReferralForUserAsync(Guid userId);

    Task<Referral> UpdateReferralStatusAsync(Guid referralId, ReferralStatus status);

    Task<Referral> ClaimReferralAsync(Guid referralId, Guid claimedByUserId);
}