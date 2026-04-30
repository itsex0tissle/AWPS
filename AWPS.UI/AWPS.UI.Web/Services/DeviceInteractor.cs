using AWPS.UI.Shared.Models;
using AWPS.UI.Shared.Services;

namespace AWPS.UI.Web.Services;

public sealed class DeviceInteractor : IDeviceInteractor
{
    #region IDeviceInteractor
    public Task<bool> Ping()
    {
        throw new NotImplementedException();
    }
    public Task<WifiStateResponse> GetWifiState()
    {
        throw new NotImplementedException();
    }
    public Task<WifiAvailableNetworkResponse[]> GetAvailableNetworks()
    {
        throw new NotImplementedException();
    }
    public Task<WifiConnectionResultResponse> ConnectToWifi(WifiCredentialsRequest request)
    {
        throw new NotImplementedException();
    }
    public Task SaveCredentials(WifiCredentialsRequest request)
    {
        throw new NotImplementedException();
    }
    #endregion
}