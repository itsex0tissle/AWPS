using AWPS.UI.Shared.Models;
using AWPS.Core.Infrastructure.MsgPack.Models;

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
    public abstract Task<TelemetryRecord[]> GetTelemetry();
    public abstract Task ClearTelemetry();
    public abstract Task<SettingsRecord> GetSettings();
    public abstract Task PutSettings(SettingsRecord request);
    public abstract Task ResetSettings();
}