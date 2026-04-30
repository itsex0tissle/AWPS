namespace AWPS.UI.Shared.Services;

public interface IServerInteractor
{
    public abstract Task<bool> Ping();
}