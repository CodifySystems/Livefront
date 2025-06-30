namespace CartonCaps.Application.Services;

/// <summary>
/// Service for generating deferred deep links for users with referral codes.
/// </summary>
public static class DeepLinkService
{
    /// <summary>
    /// Generates a deferred deep link for a user with a referral code to be shared.
    /// </summary>
    /// <param name="userId">User ID for the referral link</param>
    /// <param name="referralCode">User's Referral Code to be used for the deep link</param>
    /// <returns>Shareable Deferred Deep Link URI</returns>
    /// <example>GetDeepLink(user.UserId, user.ReferralCode)</example>
    /// <exception cref="ArgumentException"></exception>
    public static string GetDeepLink(Guid userId, string referralCode)
    {
        // Validate inputs to ensure they exist in the database
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));
        }
        if (string.IsNullOrEmpty(referralCode))
        {
            throw new ArgumentException("Referral code cannot be null or empty.", nameof(referralCode));
        }
        if (referralCode.Length < 6 || referralCode.Length > 12)
        {
            throw new ArgumentException("Referral code must be between 6 and 12 characters long.", nameof(referralCode));
        }

        // Call 3rd party service to generate the deep link
        // This is a placeholder for the actual implementation.
        // In a real application, you would call the service here and handle any exceptions.
        // For example, you might use an HTTP client to call an API that generates the deep link.

        // Simulated deep link generation
        string deepLinkId = GenerateDeepLinkId();
        string deepLink = $"https://cartoncaps.link/{deepLinkId}?referralCode={referralCode}";

        return deepLink;
    }

    /// <summary>
    /// Generates a unique deep link ID for the referral link.
    /// </summary>
    /// <returns>String value containing shareable deep link for user's referral code.</returns>
    private static string GenerateDeepLinkId()
    {
        // This method generates a unique deep link ID.
        // In a real application, you might use a GUID or another unique identifier.
        const string characters = "abcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(characters, 11)
            .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
    }
}