using System.Text.Json;
using System.Net.Http.Json;
using AWPS.UI.Shared.Models;
using AWPS.UI.Shared.Helpers;
using AWPS.UI.Shared.Services;
using AWPS.Core.Infrastructure.MsgPack.Models;
using ProGaudi.MsgPack.Light;

namespace AWPS.UI.Web.Client.Services;

public sealed class DeviceInteractor(
    [FromKeyedServices(HttpClientKey.Device)] HttpClient httpClient, 
    JsonSerializerOptions jsonOptions,
    MsgPackContext msgPackContext
) : IDeviceInteractor
{
    #region IDeviceInteractor
    public async Task<bool> Ping()
    {
        try
        {
            using HttpResponseMessage response = await httpClient.GetAsync("/");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
    public async Task<WifiStateRecord> GetWifiState()
    {
        using HttpResponseMessage response = await httpClient.GetAsync("/wifi");
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<WifiStateRecord>(await response.Content.ReadAsStreamAsync(), jsonOptions) ?? throw new Exception("Failed to deserialize the record");
    }
    public async Task<WifiAvailableNetworkRecord[]> GetAvailableNetworks()
    {
        using HttpResponseMessage response = await httpClient.GetAsync("/wifi/list");
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<WifiAvailableNetworkRecord[]>(await response.Content.ReadAsStreamAsync(), jsonOptions) ?? throw new Exception("Failed to deserialize the record");
    }
    public async Task<WifiConnectionResultRecord> ConnectToWifi(WifiCredentialsRecord request)
    {
        using HttpResponseMessage response_message = await httpClient.PostAsJsonAsync("/wifi", request);
        response_message.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<WifiConnectionResultRecord>(await response_message.Content.ReadAsStreamAsync(), jsonOptions) ?? throw new Exception("Failed to deserialize the record");
    }
    public async Task SaveCredentials(WifiCredentialsRecord request)
    {
        using HttpResponseMessage response_message = await httpClient.PostAsJsonAsync("/wifi/save", request);
        response_message.EnsureSuccessStatusCode();
    }
    public async Task<AccountRecord> GetAccount()
    {
        using HttpResponseMessage response = await httpClient.GetAsync("/account");
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<AccountRecord>(await response.Content.ReadAsStreamAsync(), jsonOptions) ?? throw new Exception("Failed to deserialize the record");
    }
    public async Task PostAccount(AccountRecord request)
    {
        using HttpResponseMessage response_message = await httpClient.PostAsJsonAsync("/account", request);
        response_message.EnsureSuccessStatusCode();
    }
    public async Task DeleteAccount()
    {
        using HttpResponseMessage response = await httpClient.DeleteAsync("/account");
        response.EnsureSuccessStatusCode();
    }
    public async Task<TelemetryRecord[]> GetTelemetry()
    {
        using HttpResponseMessage response = await httpClient.GetAsync("/telemetry");
        response.EnsureSuccessStatusCode();
        return MsgPackSerializer.Deserialize<TelemetryRecord[]>(await response.Content.ReadAsByteArrayAsync(), msgPackContext);
    }
    public async Task ClearTelemetry()
    {
        using HttpResponseMessage response = await httpClient.DeleteAsync("/telemetry");
        response.EnsureSuccessStatusCode();
    }
    public async Task<SettingsRecord> GetSettings()
    {
        using HttpResponseMessage response = await httpClient.GetAsync("/settings");
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<SettingsRecord>(await response.Content.ReadAsStreamAsync(), jsonOptions) ?? throw new Exception("Failed to deserialize the record");
    }
    public async Task PutSettings(SettingsRecord request)
    {
        using HttpResponseMessage response_message = await httpClient.PutAsJsonAsync("/settings", request);
        response_message.EnsureSuccessStatusCode();
    }
    public async Task ResetSettings()
    {
        using HttpResponseMessage response = await httpClient.DeleteAsync("/settings");
        response.EnsureSuccessStatusCode();
    }
    #endregion
}