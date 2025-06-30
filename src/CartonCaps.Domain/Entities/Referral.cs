using System.Text.Json.Serialization;
using CartonCaps.Domain.Common;
using CartonCaps.Domain.Enums;

namespace CartonCaps.Domain.Entities;

public sealed class Referral : BaseEntity
{
    /// <summary>
    /// The unique identifier for the referral.
    /// </summary>
    public Guid ReferralId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The user ID of the user who created the referral.
    /// </summary>
    public required Guid UserId { get; set; }

    /// <summary>
    /// The user ID of the user who claimed the referral, if applicable.
    /// </summary>
    public Guid? ClaimedByUserId { get; set; }

    /// <summary>
    /// The deep link that refers to the user's referral page.
    /// </summary>
    [JsonPropertyName("shareLink")]
    public required string ReferredDeepLink { get; set; }

    /// <summary>
    /// The status of the referral.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ReferralStatus Status { get; set; } = ReferralStatus.InProgress;
}