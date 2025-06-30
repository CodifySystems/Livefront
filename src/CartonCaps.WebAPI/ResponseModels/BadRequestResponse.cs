using System.Text.Json.Serialization;

namespace CartonCaps.WebAPI.ResponseModels
{
    /// <summary>
    /// Represents a bad request response model.
    /// </summary>
    public class BadRequestResponse : BaseResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BadRequestResponse"/> class.
        /// Sets the default status to "Error".
        /// </summary>
        public BadRequestResponse()
        {
            Status = "Error";
        }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        [JsonPropertyOrder(1)]
        public string Message { get; set; } = "Bad Request";
    }
}