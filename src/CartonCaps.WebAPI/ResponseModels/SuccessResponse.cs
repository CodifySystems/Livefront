using System.Text.Json.Serialization;

namespace CartonCaps.WebAPI.ResponseModels;
/// <summary>
/// Represents an API response indicating a successful operation.
/// </summary>
public class SuccessResponse : BaseResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SuccessResponse"/> class.
    /// Sets the default status to "Success".
    /// </summary>
    public SuccessResponse()
    {
        Status = "Success";
    }

    /// <summary>
    /// Gets or sets the success message.
    /// </summary>
    [JsonPropertyOrder(1)]
    public string Message { get; set; } = "Operation completed successfully.";
}