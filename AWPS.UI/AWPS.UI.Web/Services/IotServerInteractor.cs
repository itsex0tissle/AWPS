using AWPS.UI.Shared.Helpers;

namespace AWPS.UI.Web.Services;

public sealed class IotServerInteractor([FromKeyedServices(HttpClientKey.IotServer)] HttpClient httpClient)
{
    public async Task StartTrackingDevice(string device_profile_id)
    {
        await httpClient.PostAsync($"/device-profile/{device_profile_id}", null);
    }
    public async Task StopTrackingDevice(string device_profile_id)
    {
        await httpClient.DeleteAsync($"/device-profile/{device_profile_id}");
    }
}