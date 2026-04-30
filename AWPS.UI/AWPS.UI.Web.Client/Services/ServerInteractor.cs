using AWPS.UI.Shared.Helpers;
using AWPS.UI.Shared.Services;

namespace AWPS.UI.Web.Client.Services;

public sealed class ServerInteractor([FromKeyedServices(HttpClientKey.Server)] HttpClient httpClient) : IServerInteractor
{
    #region IServerInteractor
    public async Task<bool> Ping()
    {
        try
        {
            using HttpResponseMessage response = await httpClient.GetAsync("/ping");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
    #endregion
}