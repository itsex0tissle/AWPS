using AWPS.UI.Shared.Models;

namespace AWPS.UI.Shared.Services;

public interface IDeviceInteractor
{
    public abstract Task<bool> Ping();
    public abstract Task<WifiStateResponse> GetWifiState();
    public abstract Task<WifiAvailableNetworkResponse[]> GetAvailableNetworks();
    public abstract Task<WifiConnectionResultResponse> ConnectToWifi(WifiCredentialsRequest request);
    public abstract Task SaveCredentials(WifiCredentialsRequest request);
}