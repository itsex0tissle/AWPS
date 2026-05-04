using System.Diagnostics.CodeAnalysis;

namespace AWPS.UI.Models;

public sealed class AccessToken
{
    public required string Email { get; set; }
    public required TokenResponse TokenResponse { get; set; }
    public required DateTime ExpireDate { get; set; }

    public AccessToken()
    {

    }
    [SetsRequiredMembers] public AccessToken(TokenResponse token, string email)
    {
        TokenResponse = token;
        Email = email;
        ExpireDate = DateTime.UtcNow.AddSeconds(token.ExpiresIn);
    }
}