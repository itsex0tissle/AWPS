using AWPS.Core.Infrastructure.MsgPack.Models;
using AWPS.UI.Shared.Helpers;
using AWPS.UI.Shared.Models;
using AWPS.UI.Shared.Services;
using ProGaudi.MsgPack.Light;
using System.Text.Json;

namespace AWPS.UI.Services;

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
        string json = JsonSerializer.Serialize(request);
        using HttpResponseMessage response_message = await httpClient.PostAsync("/wifi", new StringContent(json));
        response_message.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<WifiConnectionResultRecord>(await response_message.Content.ReadAsStreamAsync(), jsonOptions) ?? throw new Exception("Failed to deserialize the record");
    }
    public async Task SaveCredentials(WifiCredentialsRecord request)
    {
        string json = JsonSerializer.Serialize(request);
        using HttpResponseMessage response_message = await httpClient.PostAsync("/wifi/save", new StringContent(json));
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
        string json = JsonSerializer.Serialize(request);
        using HttpResponseMessage response_message = await httpClient.PostAsync("/account", new StringContent(json));
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
        string json = JsonSerializer.Serialize(request);
        using HttpResponseMessage response_message = await httpClient.PutAsync("/settings", new StringContent(json));
        response_message.EnsureSuccessStatusCode();
    }
    public async Task ResetSettings()
    {
        using HttpResponseMessage response = await httpClient.DeleteAsync("/settings");
        response.EnsureSuccessStatusCode();
    }
    #endregion
}