using System.Text.Json.Serialization;

namespace CartonCaps.Domain.Enums;

/// <summary>
/// Enumeration representing the status of a referral.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReferralStatus
{
    /// <summary>
    /// The referral is pending and has not yet been used.
    /// </summary>
    InProgress,

    /// <summary>
    /// The referral has been used successfully completed.
    /// </summary>
    Completed,

    /// <summary>
    /// The referral has abandoned and can no longer be used.
    /// </summary>
    Abandoned
}