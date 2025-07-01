using System.Text.Json.Serialization;

namespace CartonCaps.WebAPI.ResponseModels;
/// <summary>
/// Represents an unhandled error response model.
/// </summary>
public class ErrorResponse : BaseResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorResponse"/> class.
    /// Sets the default status to "Error".
    /// </summary>
    public ErrorResponse()
    {
        Status = "Error";
    }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    [JsonPropertyOrder(1)]
    public string Message { get; set; } = "An unexpected error occurred.";

    /// <summary>
    /// Gets or sets additional details about the error.
    /// </summary>
    [JsonPropertyOrder(2)]
    public string Details { get; set; } = string.Empty;
}