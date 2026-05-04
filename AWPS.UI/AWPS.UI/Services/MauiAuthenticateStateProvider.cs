using AWPS.UI.Models;
using System.Text.Json;
using System.Net.Http.Json;
using AWPS.UI.Shared.Helpers;
using System.Security.Claims;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Authorization;

namespace AWPS.UI.Services;

public sealed class MauiAuthenticationStateProvider([FromKeyedServices(HttpClientKey.Server)] HttpClient httpClient) : AuthenticationStateProvider
{
    #region Static
    private const int TokenExpirationBuffer = 30; //minutes
    private const string AuthenticationType = "Bearer";

    private static ClaimsPrincipal DefaultUser { get; set; } = new(new ClaimsIdentity());
    private static Task<AuthenticationState> DefaultAuthState { get; set; } = Task.FromResult(new AuthenticationState(DefaultUser));

    private static ClaimsPrincipal CreateAuthenticatedUser(string email)
    {
        Claim[] claims = [new Claim(ClaimTypes.Name, email)];
        ClaimsIdentity identity = new(claims, MauiAuthenticationStateProvider.AuthenticationType);
        return new ClaimsPrincipal(identity);
    }
    #endregion

    #region Instance
    private AccessToken? AccessToken
    {
        get => field;
        set
        {
            field = value;
            if(value is not null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(MauiAuthenticationStateProvider.AuthenticationType, value.TokenResponse.AccessToken);
            }
            else
            {
                httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }
    }
    private Task<AuthenticationState> CurrentAuthState { get; set; } = MauiAuthenticationStateProvider.DefaultAuthState;

    private async Task<AccessToken?> RefreshTokenAsync(AccessToken token)
    {
        try
        {
            using HttpResponseMessage response = await httpClient.PostAsJsonAsync("/identity/refresh", new { token.TokenResponse.RefreshToken });
            if (response.IsSuccessStatusCode is true)
            {
                if (JsonSerializer.Deserialize<TokenResponse>(await response.Content.ReadAsStringAsync()) is not TokenResponse token_response)
                {
                    return null;
                }
                token = new AccessToken(token_response, token.Email);
                await TokenStorage.SaveTokenAsync(token);
                return token;
            }
        }
        catch { }
        return null;
    }
    private async Task<bool> RefreshAccessTokenAsync(AccessToken token)
    {
        AccessToken = await RefreshTokenAsync(token);
        return AccessToken is not null;
    }
    private async Task<bool> UpdateAndValidateAccessTokenAsync()
    {
        DateTime expire_limit = DateTime.UtcNow.AddMinutes(MauiAuthenticationStateProvider.TokenExpirationBuffer);
        if(AccessToken is null || expire_limit > AccessToken.ExpireDate)
        {
            AccessToken = await TokenStorage.GetTokenAsync();
        }
        if(AccessToken is null)
        {
            return false;
        }
        if(expire_limit >= AccessToken.ExpireDate)
        {
            return await RefreshAccessTokenAsync(AccessToken);
        }
        return true;
    }
    private async Task<AuthenticationState> CreateAuthenticationStateAsync()
    {
        ClaimsPrincipal user = MauiAuthenticationStateProvider.DefaultUser;
        if (await UpdateAndValidateAccessTokenAsync() is true)
        {
            user = CreateAuthenticatedUser(AccessToken!.Email);
        }
        return new AuthenticationState(user);
    }
    public async Task LoginAsync(AccessToken token, bool persistent)
    {
        AccessToken = token;
        if (persistent is true)
        {
            await TokenStorage.SaveTokenAsync(token);
        }
        else
        {
            ClaimsPrincipal user = CreateAuthenticatedUser(AccessToken.Email);
            CurrentAuthState = Task.FromResult(new AuthenticationState(user));
            base.NotifyAuthenticationStateChanged(CurrentAuthState);           
        }
    }
    public void Logout()
    {
        CurrentAuthState = MauiAuthenticationStateProvider.DefaultAuthState;
        AccessToken = null;
        TokenStorage.RemoveToken();
        base.NotifyAuthenticationStateChanged(CurrentAuthState);
    }
    #endregion

    #region AuthenticationStateProvider
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if(CurrentAuthState != MauiAuthenticationStateProvider.DefaultAuthState)
        {
            return CurrentAuthState;
        }
        CurrentAuthState = CreateAuthenticationStateAsync();
        base.NotifyAuthenticationStateChanged(CurrentAuthState);
        return CurrentAuthState;
    }
    #endregion
}