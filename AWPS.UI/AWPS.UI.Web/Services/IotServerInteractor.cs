namespace AWPS.UI.Web.Services;

public sealed class IotServerInteractor(IHttpClientFactory httpClientFactory)
{
    public async Task SubscribeAsync(string device_profile_id)
    {
        try
        {
            using HttpClient client = httpClientFactory.CreateClient("IotServer");
            await client.PostAsync($"/device-profile/{device_profile_id}", null);
        }
        catch { }
    }
    public async Task UnsubscribeAsync(string device_profile_id)
    {
        try
        {
            using HttpClient client = httpClientFactory.CreateClient("IotServer");
            await client.DeleteAsync($"/device-profile/{device_profile_id}");
        }
        catch { }
    }
}