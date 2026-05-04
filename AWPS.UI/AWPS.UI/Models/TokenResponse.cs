using System.Text.Json.Serialization;

namespace AWPS.UI.Models;

public sealed class TokenResponse
{
    [JsonPropertyName("tokenType")]
    public required string TokenType { get; set; }

    [JsonPropertyName("accessToken")]
    public required string AccessToken { get; set; }

    [JsonPropertyName("expiresIn")]
    public required long ExpiresIn { get; set; } = 0;

    [JsonPropertyName("refreshToken")]
    public required string RefreshToken { get; set; }
}