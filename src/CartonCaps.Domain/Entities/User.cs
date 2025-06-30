using CartonCaps.Domain.Common;

namespace CartonCaps.Domain.Entities;

public sealed class User : BaseEntity
{
    public Guid UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    // Represents the user's full name in a short format, e.g., "Josie P."
    public string ShortDisplayName
    {
        get => $"{FirstName} {LastName.First()}.";
    }
    public required string Email { get; set; }
    // ReferralCode is a unique code generated for each user to share with others
    public required string ReferralCode { get; set; }
    // ReferralId is used to link the user to their referral
    public Guid ReferralId { get; set; }
}