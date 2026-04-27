namespace AWPS.UI.Shared.MsgPack.Models
{
    public sealed class WifiCredentialsRequest
    {
        public string SSID { get; set; } = "";
        public string Password { get; set; } = "";
    }
}