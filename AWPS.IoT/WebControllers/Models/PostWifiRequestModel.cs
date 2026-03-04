#nullable enable

namespace AWPS.IoT.WebControllers.Models
{
    public sealed class PostWifiRequestModel
    {
        public string SSID { get; set; } = "";
        public string Password { get; set; } = "";
    }
}