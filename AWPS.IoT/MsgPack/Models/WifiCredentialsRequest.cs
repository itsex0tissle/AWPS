namespace AWPS.IoT.MsgPack.Models
{
    public sealed class WifiCredentialsRequest
    {
        public string SSID { get; set; } = "";
        public string Password { get; set; } = "";
    }
}