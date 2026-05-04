using Ardalis.Result;

namespace AWPS.UI.Shared.Services;

public interface IAuthService
{
    public abstract Task<Result> RegisterAsync(string email, string password);
    public abstract Task<Result> LoginAsync(string email, string password, bool persistent);
    public abstract Task<Result> LogoutAsync();
}