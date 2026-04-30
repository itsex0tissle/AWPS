using AWPS.UI.Shared.Models;
using AWPS.UI.Shared.Helpers;
using ProGaudi.MsgPack.Light;
using System.Net.Http.Headers;
using AWPS.UI.Shared.Services;

namespace AWPS.UI.Web.Client.Services;

public sealed class DeviceInteractor([FromKeyedServices(HttpClientKey.Device)] HttpClient httpClient, MsgPackContext context) : IDeviceInteractor
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
    public async Task<WifiStateResponse> GetWifiState()
    {
        using HttpResponseMessage response = await httpClient.GetAsync("/wifi");
        response.EnsureSuccessStatusCode();
        return MsgPackSerializer.Deserialize<WifiStateResponse>(await response.Content.ReadAsByteArrayAsync(), context);
    }
    public async Task<WifiAvailableNetworkResponse[]> GetAvailableNetworks()
    {
        using HttpResponseMessage response = await httpClient.GetAsync("/wifi/list");
        response.EnsureSuccessStatusCode();
        return MsgPackSerializer.Deserialize<WifiAvailableNetworkResponse[]>(await response.Content.ReadAsByteArrayAsync(), context);
    }
    public async Task<WifiConnectionResultResponse> ConnectToWifi(WifiCredentialsRequest request)
    {
        byte[] data = MsgPackSerializer.Serialize(request, context);
        HttpRequestMessage request_message = new(HttpMethod.Post, "/wifi")
        {
            Content = new ByteArrayContent(data)
        };
        request_message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-msgpack");
        request_message.Content.Headers.ContentLength = data.Length;
        using HttpResponseMessage response_message = await httpClient.SendAsync(request_message);
        response_message.EnsureSuccessStatusCode();
        return MsgPackSerializer.Deserialize<WifiConnectionResultResponse>(await response_message.Content.ReadAsByteArrayAsync(), context);
    }
    public async Task SaveCredentials(WifiCredentialsRequest request)
    {
        byte[] data = MsgPackSerializer.Serialize(request, context);
        HttpRequestMessage request_message = new(HttpMethod.Post, "/wifi/save")
        {
            Content = new ByteArrayContent(data)
        };
        request_message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-msgpack");
        request_message.Content.Headers.ContentLength = data.Length;
        using HttpResponseMessage response_message = await httpClient.SendAsync(request_message);
        response_message.EnsureSuccessStatusCode();
    }
    #endregion
}