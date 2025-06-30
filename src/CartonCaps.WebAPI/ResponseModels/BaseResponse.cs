using System.Text.Json.Serialization;

namespace CartonCaps.WebAPI.ResponseModels;

/// <summary>
/// Base class for all API response models.
/// </summary>
public class BaseResponse
{
    /// <summary>
    /// Gets or sets the status of the response.
    /// </summary>
    [JsonPropertyOrder(0)]
    public string? Status { get; set; }
}