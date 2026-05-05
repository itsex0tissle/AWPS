using AWPS.UI.Shared.Models;

namespace AWPS.UI.Shared.Services;

public interface IDeviceInteractor
{
    public abstract Task<bool> Ping();
    public abstract Task<WifiStateRecord> GetWifiState();
    public abstract Task<WifiAvailableNetworkRecord[]> GetAvailableNetworks();
    public abstract Task<WifiConnectionResultRecord> ConnectToWifi(WifiCredentialsRecord request);
    public abstract Task SaveCredentials(WifiCredentialsRecord request);
    public abstract Task<AccountRecord> GetAccount();
    public abstract Task PostAccount(AccountRecord request);
    public abstract Task DeleteAccount();
}