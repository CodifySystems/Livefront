using System.Text.Json.Serialization;

namespace CartonCaps.WebAPI.ResponseModels;

public class BaseResponse
{
    /// <summary>
    /// Gets or sets the status of the response.
    /// </summary>
    [JsonPropertyOrder(0)]
    public string? Status { get; set; }
}