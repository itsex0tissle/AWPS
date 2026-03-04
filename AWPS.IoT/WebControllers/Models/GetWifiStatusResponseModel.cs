#nullable enable

namespace AWPS.IoT.WebControllers.Models
{
    public sealed class GetWifiStatusResponseModel
    {
        public string SSID { get; set; } = "";
        public bool Connected { get; set; } = false;
    }
}