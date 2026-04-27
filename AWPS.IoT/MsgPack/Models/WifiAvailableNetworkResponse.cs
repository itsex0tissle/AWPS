namespace AWPS.IoT.MsgPack.Models
{
    public sealed class WifiAvailableNetworkResponse
    {
        public string SSID { get; set; } = "";
        public byte SignalBars { get; set; }
    }
}