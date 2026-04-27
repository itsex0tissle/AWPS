namespace AWPS.UI.Shared.MsgPack.Models
{
    public sealed class WifiStateResponse
    {
        public string SSID { get; set; } = "";
        public bool Connected { get; set; }
    }
}