using AWPS.UI.Models;
using System.Text.Json;

namespace AWPS.UI.Services;

public static class TokenStorage
{
    private const string StorageKeyName = "access_token";

    public static async Task SaveTokenAsync(AccessToken access_token)
    {
        await SecureStorage.SetAsync(StorageKeyName, JsonSerializer.Serialize(access_token));
    }
    public static async Task<AccessToken?> GetTokenAsync()
    {
        if(await SecureStorage.GetAsync(StorageKeyName) is not string token_str)
        {
            return null;
        }
        return JsonSerializer.Deserialize<AccessToken>(token_str);
    }
    public static void RemoveToken()
    {
        SecureStorage.Remove(StorageKeyName);
    }
}