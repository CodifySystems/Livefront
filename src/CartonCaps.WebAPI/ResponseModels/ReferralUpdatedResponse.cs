using System.Text.Json.Serialization;
using CartonCaps.Domain.Enums;

namespace CartonCaps.WebAPI.ResponseModels;

/// <summary>
/// Response model for updating a referral.
/// </summary>
public class ReferralUpdatedResponse : BaseResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReferralUpdatedResponse"/> class.
    /// Sets the default status to "Success".
    /// </summary>
    public ReferralUpdatedResponse()
    {
        Status = "Success";
    }

    /// <summary>
    /// Gets or sets the message indicating the result of the referral creation.
    /// </summary>
    [JsonPropertyOrder(1)]
    public string Message { get; set; } = "Referral created successfully.";

    /// <summary>
    /// Gets or sets the ID of the newly created referral.
    /// </summary>
    [JsonPropertyOrder(2)]
    public Guid ReferralId { get; set; }

    /// <summary>
    /// Gets or sets the referral status after the update.
    /// </summary>
    [JsonPropertyOrder(3)]
    public ReferralStatus NewStatus { get; set; }
}