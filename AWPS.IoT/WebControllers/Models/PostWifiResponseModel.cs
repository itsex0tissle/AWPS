#nullable enable

namespace AWPS.IoT.WebControllers.Models
{
    public sealed class PostWifiResponseModel
    {
        public bool Success { get; set; } = false;
        public string Description { get; set; } = "";
    }
}