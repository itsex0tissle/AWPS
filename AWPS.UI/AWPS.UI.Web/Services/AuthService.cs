using Ardalis.Result;
using AWPS.UI.Shared.Services;
using AWPS.Core.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace AWPS.UI.Web.Services;

public sealed class AuthService(
    IUserStore<ApplicationUserEntity> userStore, 
    UserManager<ApplicationUserEntity> userManager, 
    SignInManager<ApplicationUserEntity> signInManager
) : IAuthService
{
    #region IAuthService
    public async Task<Result> RegisterAsync(string email, string password)
    {
        try
        {
            ApplicationUserEntity user = new();
            await userStore.SetUserNameAsync(user, email, CancellationToken.None);
            await GetEmailStore().SetEmailAsync(user, email, CancellationToken.None);
            IdentityResult result = await userManager.CreateAsync(user, password);
            if(result.Succeeded is true)
            {
                return Result.Success();
            }
            return Result.Error(new ErrorList(result.Errors.Select(error => error.Description)));

            IUserEmailStore<ApplicationUserEntity> GetEmailStore()
            {
                return (IUserEmailStore<ApplicationUserEntity>)userStore;
            }
        }
        catch(Exception exc)
        {
            return Result.CriticalError(exc.Message);
        }
    }
    public async Task<Result> LoginAsync(string email, string password, bool persistent)
    {
        try
        {
            SignInResult result = await signInManager.PasswordSignInAsync(email, password, persistent, lockoutOnFailure: false);
            if(result.Succeeded is true)
            {
                return Result.Success();
            }
            if(result.IsLockedOut is true)
            {
                return Result.Error(new ErrorList(["User account is locked out."]));
            }
            if(result.IsNotAllowed is true)
            {
                return Result.Error(new ErrorList(["User account is not allowed to sign in."]));
            }
            return Result.Error(new ErrorList(["Invalid email or password."]));
        }
        catch(Exception exc)
        {
            return Result.CriticalError(exc.Message);
        }
    }
    public async Task<Result> LogoutAsync()
    {
        try
        {
            await signInManager.SignOutAsync();
            return Result.Success();
        }
        catch(Exception exc)
        {
            return Result.CriticalError(exc.Message);
        }
    }
    #endregion
}