namespace AWPS.IoT.MsgPack.Models
{
    public sealed class WifiAvailableNetworkRecord
    {
        public string SSID { get; set; } = "";
        public byte SignalBars { get; set; }
    }
}