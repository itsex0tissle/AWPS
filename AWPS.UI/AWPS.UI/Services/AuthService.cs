using Ardalis.Result;
using AWPS.UI.Models;
using System.Text.Json;
using System.Net.Http.Json;
using AWPS.UI.Shared.Helpers;
using AWPS.UI.Shared.Services;

namespace AWPS.UI.Services;

public sealed class AuthService([FromKeyedServices(HttpClientKey.Server)] HttpClient httpClient, MauiAuthenticationStateProvider masp) : IAuthService
{
    #region IAuthService
    public async Task<Result> RegisterAsync(string email, string password)
    {
        try
        {
            using HttpResponseMessage response = await httpClient.PostAsJsonAsync("identity/register", new { Email = email, Password = password });
            if (response.IsSuccessStatusCode is true)
            {
                return Result.Success();
            }
            else
            {
                try
                {
                    if (JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.TryGetProperty("errors", out JsonElement json_dict) is false)
                    {
                        return Result.CriticalError("Failed to get 'errors' property from json response.");
                    }
                    if (json_dict.Deserialize<Dictionary<string, string[]>>()?.SelectMany(pair => pair.Value) is not IEnumerable<string> errors)
                    {
                        return Result.CriticalError("Failed to deserialize 'errors' property from json response.");
                    }
                    return Result.Error(new ErrorList(errors));
                }
                catch (JsonException)
                {
                    return Result.CriticalError("Response is not a valid JSON.");
                }
            }
        }
        catch (HttpRequestException)
        {
            return Result.CriticalError(["Failed to connect to the server."]);
        }
        catch (Exception exc)
        {
            return Result.CriticalError(exc.Message);
        }
    }
    public async Task<Result> LoginAsync(string email, string password, bool persistent)
    {
        try
        {
            using HttpResponseMessage response = await httpClient.PostAsJsonAsync("identity/login", new { Email = email, Password = password });
            if (response.IsSuccessStatusCode is true)
            {
                if(JsonSerializer.Deserialize<TokenResponse>(await response.Content.ReadAsStringAsync()) is not TokenResponse token_response)
                {
                    return Result.CriticalError("Failed to deserialize token response.");
                }
                AccessToken token = new(token_response, email);
                await masp.LoginAsync(token, persistent);
                return Result.Success();
            }
            else
            {
                try
                {
                    if (JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement.TryGetProperty("detail", out JsonElement json_str) is false)
                    {
                        return Result.CriticalError("Failed to get 'detail' property from json response.");
                    }
                    if (json_str.GetString() is not string error)
                    {
                        return Result.CriticalError("Failed to parse 'detail' property from json response.");
                    }
                    return error switch
                    {
                        "LockedOut" => Result.Error("User account is locked out."),
                        "NotAllowed" => Result.Error("User account is not allowed to sign in."),
                        _ => Result.Error("Invalid email or password.")
                    };
                }
                catch (JsonException)
                {
                    return Result.Error("Response is not a valid JSON.");
                }
            }
        }
        catch(HttpRequestException)
        {
            return Result.Error("Failed to connect to the server.");
        }
        catch(Exception exc)
        {
            return Result.Error(exc.Message);
        }
    }
    public Task<Result> LogoutAsync()
    {
        try
        {
            masp.Logout();
            return Task.FromResult(Result.Success());
        }
        catch(Exception exc)
        {
            return Task.FromResult(Result.CriticalError(exc.Message));
        }
    }
    #endregion
}