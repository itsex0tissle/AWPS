using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Authorization;

namespace AWPS.UI.Web.Services;

internal sealed class IdentityRevalidatingAuthenticationStateProvider(
    ILoggerFactory loggerFactory, 
    IServiceScopeFactory scopeFactory, 
    IOptions<IdentityOptions> options
) : RevalidatingServerAuthenticationStateProvider(loggerFactory)
{
    #region Instance
    private async Task<bool> ValidateSecurityStampAsync(UserManager<IdentityUser> user_manager, ClaimsPrincipal principal)
    {
        if(await user_manager.GetUserAsync(principal) is not IdentityUser user)
        {
            return false;
        }
        else if(user_manager.SupportsUserSecurityStamp is false)
        {
            return true;
        }
        else
        {
            string? principal_stamp = principal.FindFirstValue(options.Value.ClaimsIdentity.SecurityStampClaimType);
            string? user_stamp = await user_manager.GetSecurityStampAsync(user);
            return principal_stamp == user_stamp;
        }
    }
    #endregion

    #region RevalidatingServerAuthenticationStateProvider
    protected override TimeSpan RevalidationInterval
    {
        get => TimeSpan.FromMinutes(30);
    }

    protected override async Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
        await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
        UserManager<IdentityUser> user_manager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        return await ValidateSecurityStampAsync(user_manager, authenticationState.User);
    }
    #endregion
}