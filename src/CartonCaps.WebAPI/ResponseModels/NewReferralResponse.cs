using System.Text.Json.Serialization;

namespace CartonCaps.WebAPI.ResponseModels;

public class NewReferralResponse : BaseResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NewReferralResponse"/> class.
    /// Sets the default status to "Success".
    /// </summary>
    public NewReferralResponse()
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
    /// Gets or sets the share link for the referral.
    /// </summary>
    [JsonPropertyOrder(3)]
    public string ShareLink { get; set; } = string.Empty;
}