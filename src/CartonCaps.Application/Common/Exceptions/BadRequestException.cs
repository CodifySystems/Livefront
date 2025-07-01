namespace CartonCaps.Application.Common.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a bad request is made to the server.
/// </summary>
public class BadRequestException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public BadRequestException(string message) : base(message)
    {
    }
}